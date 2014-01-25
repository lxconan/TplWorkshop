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
            var task = new Task<string>(() => V.Start("task 1").Sleep(1).Finish("hello"));
            task.Start();
            Assert.Equal("hello", task.Result);
        }

        [Fact]
        public void create_task_by_factory()
        {
            var task = Task.Factory.StartNew(() => V.Start().Sleep(1).Finish("hello"));
            Assert.Equal("hello", task.Result);
        }

        [Fact]
        public void wait_for_one_task()
        {
            var task = Task.Factory.StartNew(() => V.Start().Sleep(1).Finish("hello"));
            task.Wait();
            Assert.True(task.IsCompleted);
            Assert.Equal("hello", task.Result);
        }

        [Fact]
        public void wait_for_multiple_tasks()
        {
            Task[] tasks =
            {
                Task.Factory.StartNew(() => V.Start("task 1").Sleep(1).Finish()),
                Task.Factory.StartNew(() => V.Start("task 2").Sleep(1).Finish()),
                Task.Factory.StartNew(() => V.Start("task 3").Sleep(1).Finish())
            };

            Task.WaitAll(tasks);
            Assert.True(tasks.All(task => task.IsCompleted));
        }

        [Fact]
        public void wait_for_first_complete_task()
        {
            Task[] tasks =
            {
                Task.Factory.StartNew(() => V.Start("task 1").Sleep(3).Finish()),
                Task.Factory.StartNew(() => V.Start("task 2").Sleep(2).Finish()),
                Task.Factory.StartNew(() => V.Start("task 3").Sleep(1).Finish())
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
                s => V.Start("task 1").Sleep(1).Finish(s), state);

            task.Wait();
            Assert.Same(state, task.Result);
        }

        [Fact]
        public void create_child_task()
        {
            Task task = Task.Factory.StartNew(
                () =>
                {
                    using (V.Start("task 1").Sleep(1))
                    {
                        Task.Factory.StartNew(
                            () => V.Start("task 2").Sleep(3).Dispose(),
                            TaskCreationOptions.AttachedToParent);
                    }
                });

            task.Wait();
        }

        [Fact]
        public void handle_aggregate_exception()
        {
            Task[] tasks =
            {
                Task.Factory.StartNew(() =>
                    V.Start("task 1").Sleep(1)
                        .Finish(new ArgumentException("ArgumentException"))),
                Task.Factory.StartNew(() =>
                    V.Start("task 2").Sleep(2)
                        .Finish(new InvalidOperationException("InvalidOperationException")))
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
                () =>
                {
                    using (var tracer = V.Start("task 1"))
                    {
                        tracer.Sleep(2);
                        if (cancellationToken.IsCancellationRequested)
                        {
                            throw new OperationCanceledException(cancellationToken);
                        }

                        tracer.Sleep(3);
                    }
                },
                cancellationToken);

            Thread.Sleep(500);
            cancellationTokenSource.Cancel();

            try
            {
                task.Wait();
            }
            catch (AggregateException)
            {
                Trace.WriteLine("Exception caught!");
            }

            Assert.True(task.IsCanceled);
        }
    }
}