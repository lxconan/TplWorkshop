using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TplWorkshop.Facts.BasicOps
{
    public class CreateTaskFacts : TimeTracingFacts
    {
        [Fact]
        public void create_task_by_ctor()
        {
            var task = new Task<string>(LongRunningOperations.WaitFor1SecAndReturnHello);
            task.Start();
            Assert.Equal("hello", task.Result);
        }

        [Fact]
        public void create_task_by_factory()
        {
            var task = Task.Factory.StartNew<string>(LongRunningOperations.WaitFor1SecAndReturnHello);
            Assert.Equal("hello", task.Result);
        }

        [Fact]
        public void wait_for_one_task()
        {
            var task = Task.Factory.StartNew<string>(LongRunningOperations.WaitFor1SecAndReturnHello);
            task.Wait();
            Assert.True(task.IsCompleted);
            Assert.Equal("hello", task.Result);
        }

        [Fact]
        public void wait_for_multiple_tasks()
        {
            Task[] tasks =
            {
                Task.Factory.StartNew<string>(LongRunningOperations.WaitFor1SecAndReturnHello),
                Task.Factory.StartNew<string>(LongRunningOperations.WaitFor1SecAndReturnHello),
                Task.Factory.StartNew<string>(LongRunningOperations.WaitFor1SecAndReturnHello)
            };

            Task.WaitAll(tasks);
            Assert.True(tasks.All(task => task.IsCompleted));
        }

        [Fact]
        public void wait_for_first_complete_task()
        {
            Task[] tasks =
            {
                Task.Factory.StartNew<string>(LongRunningOperations.WaitFor3SecAndReturnHello),
                Task.Factory.StartNew<string>(LongRunningOperations.WaitFor2SecAndReturnHello),
                Task.Factory.StartNew<string>(LongRunningOperations.WaitFor1SecAndReturnHello)
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
            Task<object> task = Task.Factory.StartNew<object>(LongRunningOperations.WaitFor1SecAndEcho, state);
            task.Wait();
            Assert.Same(state, task.Result);
        }

        [Fact]
        public void create_child_task()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(
                    LongRunningOperations.WaitFor3Sec,
                    TaskCreationOptions.AttachedToParent);
                LongRunningOperations.WaitFor1Sec();
            });

            task.Wait();
        }

        [Fact]
        public void handle_aggregate_exception()
        {
            Task[] tasks =
            {
                Task.Factory.StartNew(() => { throw new ArgumentException("ArgumentException"); }),
                Task.Factory.StartNew(() => { throw new InvalidOperationException("InvalidOperationException"); })
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
                    LongRunningOperations.WaitFor3Sec();
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(cancellationToken);
                    }

                    LongRunningOperations.WaitFor1Sec();
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