using System;
using System.Linq;
using System.Threading.Tasks;
using TplWorkshop.Util;
using Xunit;

namespace TplWorkshop.Facts.BasicOps
{
    public class Continuation : VisualizedTaskFact
    {
        [Fact]
        public void continue_when_task_complete()
        {
            Task task = Task.Factory
                .StartNew(() =>
                {
                    V.Start("task 1").Sleep(2).Finish();
                    V.Hint("task 1 exection finished.");
                })
                .ContinueWith(t =>
                {
                    V.Start("task 2").Sleep(1).Finish();
                    V.Hint("task 2 execting finished.");
                })
                .ContinueWith(t =>
                {
                    V.Start("task 3").Sleep(2).Finish();
                    V.Hint("task 3 executing finished.");
                });

            task.Wait();
            V.Hint("Waiting for task 3 by using task.Wait() complete.");
        }

        [Fact]
        public void continue_and_get_previous_result()
        {
            var task = Task.Factory
                .StartNew(() => V.Start("task 1").Sleep(1).Finish("hello from task 1"))
                .ContinueWith(resultFromTask1 =>
                {
                    V.Hint("We can get result from task 1: " + resultFromTask1.Result);
                    return V.Start("task 2").Sleep(1).Finish("hello from task 2");
                })
                .ContinueWith(resultFromTask2 =>
                {
                    V.Hint("We can get result from task 2: " + resultFromTask2.Result);
                    V.Start("task 3").Sleep(1).Finish();
                });

            task.Wait();
        }

        [Fact]
        public void continue_multiple_tasks_using_when_all()
        {
            var tasks = new[]
            {
                Task.Factory.StartNew(() => V.Start("task 1").Sleep(1).Finish()),
                Task.Factory.StartNew(() => V.Start("task 2").Sleep(3).Finish())
            };

            Task task = Task.Factory
                .ContinueWhenAll(tasks, completedTask => V.Start("task 3").Sleep(1).Finish());
            task.Wait();
            V.Hint("Waiting for 'task 3' by using Task.Wait() complete.");
        }

        [Fact]
        public void continue_multiple_tasks_using_when_any()
        {
            var tasks = new[]
            {
                Task.Factory.StartNew(() => V.Start("task 1").Sleep(1).Finish("hello 1")),
                Task.Factory.StartNew(() => V.Start("task 2").Sleep(3).Finish("hello 2"))
            };

            Task task = Task.Factory.ContinueWhenAny(tasks, completedTask =>
            {
                V.Hint("The task with return value '" + completedTask.Result + "' is complete.");
                V.Start("task 3").Sleep(1).Finish();
            });

            Task.WaitAll(tasks.Concat(new[] {task}).ToArray());
            V.Hint("Waiting all tasks complete.");
        }

        [Fact]
        public void continue_broadcasting()
        {
            Task task = Task.Factory.StartNew(() => V.Start("task 1").Sleep(3).Finish());
            Task subTask1 = task.ContinueWith(t => V.Start("task 2").Sleep(1).Finish());
            Task subTask2 = task.ContinueWith(t => V.Start("task 3").Sleep(2).Finish());
            Task.WaitAll(subTask1, subTask2);
            V.Hint("Waiting all tasks complete.");
        }

        [Fact]
        public void continue_task_for_certain_condition()
        {
            Task task = Task.Factory
                .StartNew(() => V.Start("task 1").Sleep(1).FinishError(new ArgumentException()));
            var subTask1 = task
                .ContinueWith(t => V.Start("task success").Sleep(2).Finish(), TaskContinuationOptions.OnlyOnRanToCompletion);
            var subTask2 = task
                .ContinueWith(t => V.Start("task failure").Sleep(2).Finish(), TaskContinuationOptions.OnlyOnFaulted);

            var cancelledTaskError = Assert.Throws<AggregateException>(() => subTask1.Wait());
            Assert.Equal(1, cancelledTaskError.InnerExceptions.Count);
            Assert.IsType<TaskCanceledException>(cancelledTaskError.InnerExceptions.Single());
            subTask2.Wait();
        }
    }
}