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
        private static List<ITaskVisualizerRecord> GetAllRecords(ITaskVisualizerReport report)
        {
            return report.Threads.SelectMany(r => r.Tasks).ToList();
        }
            
        [Fact]
        public void should_add_new_record_after_action_running()
        {
            var taskVisualizer = new TaskVisualizer();
            taskVisualizer.RunFunc(TimeSpan.FromSeconds(1), () => new object());

            ITaskVisualizerReport report = taskVisualizer.GetReport();
            Assert.Equal(1, GetAllRecords(report).Count);
        }

        [Fact]
        public void should_record_name()
        {
            var taskVisualizer = new TaskVisualizer();
            const string expected = "task";
            taskVisualizer.RunFunc(TimeSpan.FromSeconds(1), () => new object(), expected);

            ITaskVisualizerReport report = taskVisualizer.GetReport();
            ITaskVisualizerRecord record = GetAllRecords(report).Single();

            Assert.Equal(expected, record.Name);
        }

        [Fact]
        public void should_delay_and_execute()
        {
            var taskVisualizer = new TaskVisualizer();
            TimeSpan delayDuration = TimeSpan.FromSeconds(3);
            taskVisualizer.RunFunc(delayDuration, () => new object());

            ITaskVisualizerReport report = taskVisualizer.GetReport();
            ITaskVisualizerRecord record = GetAllRecords(report).Single();

            Assert.True(record.Duration >= delayDuration.TotalSeconds);
        }

        [Fact]
        public void should_get_result()
        {
            var taskVisualizer = new TaskVisualizer();
            const string desiredResult = "hello";
            taskVisualizer.RunFunc(TimeSpan.FromSeconds(1), () => desiredResult);

            ITaskVisualizerReport report = taskVisualizer.GetReport();
            ITaskVisualizerRecord record = GetAllRecords(report).Single();

            Assert.Equal((int)TaskStatus.RanToCompletion, record.TaskStatus);
            Assert.Equal("String: hello", record.TaskResult);
        }

        [Fact]
        public void should_get_exception()
        {
            var taskVisualizer = new TaskVisualizer();

            Assert.Throws<ArgumentException>(() =>
            {
                taskVisualizer.RunFunc<object>(
                    TimeSpan.FromSeconds(1),
                    () => { throw new ArgumentException(); });
            });

            ITaskVisualizerRecord record = GetAllRecords(taskVisualizer.GetReport()).Single();
            
            Assert.NotNull(record.TaskError);
            Assert.Equal((int)TaskStatus.Faulted, record.TaskStatus);
        }
    }
}