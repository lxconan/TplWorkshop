using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TplWorkshop.Util.Visualizer;
using Xunit;

namespace TplWorkshop.Util.Facts
{
    public class TaskVisualizerFact
    {
        private static List<TaskRunningRecord> GetAllRecords(ITaskVisualizerReport report)
        {
            return report.Data.SelectMany(r => r.Value).ToList();
        }
            
        [Fact]
        public void should_add_new_record_after_action_running()
        {
            var taskVisualizer = new TaskVisualizer();
            taskVisualizer.RunAction(TimeSpan.FromSeconds(1), () => new object());

            ITaskVisualizerReport report = taskVisualizer.GetReport();
            Assert.Equal(1, GetAllRecords(report).Count);
        }

        [Fact]
        public void should_record_name()
        {
            var taskVisualizer = new TaskVisualizer();
            const string expected = "task";
            taskVisualizer.RunAction(TimeSpan.FromSeconds(1), () => new object(), expected);

            ITaskVisualizerReport report = taskVisualizer.GetReport();
            TaskRunningRecord record = GetAllRecords(report).Single();

            Assert.Equal(expected, record.Name);
        }

        [Fact]
        public void should_delay_and_execute()
        {
            var taskVisualizer = new TaskVisualizer();
            TimeSpan delayDuration = TimeSpan.FromSeconds(3);
            taskVisualizer.RunAction(delayDuration, () => new object());

            ITaskVisualizerReport report = taskVisualizer.GetReport();
            TaskRunningRecord record = GetAllRecords(report).Single();

            Assert.True(record.EndTime - record.StartTime >= delayDuration);
        }

        [Fact]
        public void should_get_result()
        {
            var taskVisualizer = new TaskVisualizer();
            var desiredResult = new object();
            taskVisualizer.RunAction(TimeSpan.FromSeconds(1), () => desiredResult);

            ITaskVisualizerReport report = taskVisualizer.GetReport();
            TaskRunningRecord record = GetAllRecords(report).Single();

            Assert.Equal(TaskStatus.RanToCompletion, record.Status);
            Assert.Same(desiredResult, record.TaskResult);
        }

        [Fact]
        public void should_get_exception()
        {
            var taskVisualizer = new TaskVisualizer();

            var exception = Assert.Throws<ArgumentException>(() =>
            {
                taskVisualizer.RunAction<object>(
                    TimeSpan.FromSeconds(1),
                    () => { throw new ArgumentException(); });
            });

            TaskRunningRecord record = GetAllRecords(taskVisualizer.GetReport()).Single();
            
            Assert.Same(exception, record.CapturedError);
            Assert.Equal(TaskStatus.Faulted, record.Status);
        }
    }
}