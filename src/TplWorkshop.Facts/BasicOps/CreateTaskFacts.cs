using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TplWorkshop.Util;
using Xunit;

namespace TplWorkshop.Facts.BasicOps
{
    public class CreateTaskFacts : VisualizedTaskFact
    {
        [Fact]
        public void create_task_by_ctor()
        {
            var task = new Task<string>(() => Visualizer.Wait1SecAndReturnHello("task 1"));
            task.Start();
            Assert.Equal("hello", task.Result);
        }

        [Fact]
        public void create_task_by_factory()
        {
            var task = Task.Factory.StartNew(() => Visualizer.Wait1SecAndReturnHello("task 1"));
            Assert.Equal("hello", task.Result);
        }

        [Fact]
        public void wait_for_one_task()
        {
            var task = Task.Factory.StartNew(() => Visualizer.Wait1SecAndReturnHello("task 1"));
            task.Wait();
            Assert.True(task.IsCompleted);
            Assert.Equal("hello", task.Result);
        }

        [Fact]
        public void wait_for_multiple_tasks()
        {
            Task[] tasks =
            {
                Task.Factory.StartNew(() => Visualizer.Wait1SecAndReturnHello("task 1")),
                Task.Factory.StartNew(() => Visualizer.Wait1SecAndReturnHello("task 2")),
                Task.Factory.StartNew(() => Visualizer.Wait1SecAndReturnHello("task 3"))
            };

            Task.WaitAll(tasks);
            Assert.True(tasks.All(task => task.IsCompleted));
        }

        [Fact]
        public void wait_for_first_complete_task()
        {
            Task[] tasks =
            {
                Task.Factory.StartNew(() => Visualizer.Wait3SecAndReturnHello("task 1")),
                Task.Factory.StartNew(() => Visualizer.Wait2SecAndReturnHello("task 2")),
                Task.Factory.StartNew(() => Visualizer.Wait1SecAndReturnHello("task 3"))
            };

            int completeIndex = Task.WaitAny(tasks);

            Trace.WriteLine("First completed task's index is : " + completeIndex);
            Assert.True(tasks[completeIndex].IsCompleted);

            Task.WaitAll(tasks);
        }

        [Fact]
        public void passing_data_to_task()
        {
            var state = new Object();
            Task<object> task = Task.Factory.StartNew(
                s => Visualizer.Wait1SecAndEcho(s, "task 1"), state);

            task.Wait();
            Assert.Same(state, task.Result);
        }

        [Fact]
        public void create_child_task()
        {
            Task task = Task.Factory.StartNew(
                () => Visualizer.RunAction(
                    TimeSpan.FromSeconds(1),
                    () => Task.Factory.StartNew(
                        () => Visualizer.Wait3SecAndReturnHello("task 2"),
                        TaskCreationOptions.AttachedToParent),
                    "task 1"));

            task.Wait();
        }

        [Fact]
        public void handle_aggregate_exception()
        {
            Task[] tasks =
            {
                Task.Factory.StartNew(() => 
                    Visualizer.RunAction(
                        TimeSpan.FromSeconds(1), 
                        () => {throw new ArgumentException("ArgumentException");}, 
                        "task 1")),
                Task.Factory.StartNew(() => 
                    Visualizer.RunAction(
                        TimeSpan.FromSeconds(1), 
                        () => {throw new InvalidOperationException("InvalidOperationException");}, 
                        "task 2"))
            };

            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Trace.WriteLine(exception.Message);
                }
            }
        }

        [Fact]
        public void cancelling_task()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Task task = Task.Factory.StartNew(
                () => Visualizer.RunAction(
                    TimeSpan.FromSeconds(1),
                    () =>
                    {
                        Thread.Sleep(3000);
                        if (cancellationToken.IsCancellationRequested)
                        {
                            throw new OperationCanceledException(cancellationToken);
                        }

                        Thread.Sleep(1000);
                    },
                    "task 1"),
                cancellationToken);

            Thread.Sleep(500);
            cancellationTokenSource.Cancel();

            try
            {
                task.Wait(cancellationToken);
            }
            catch (AggregateException)
            {
                Trace.WriteLine("Exception caught!");
            }

            Assert.True(task.IsCanceled);
        }
    }
}