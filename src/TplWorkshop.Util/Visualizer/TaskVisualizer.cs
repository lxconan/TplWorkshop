using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace TplWorkshop.Util.Visualizer
{
    internal class TaskVisualizer : ITaskVisualizer
    {
        private readonly ConcurrentBag<TaskRunningRecord> m_logs;

        public TaskVisualizer()
        {
            m_logs = new ConcurrentBag<TaskRunningRecord>();
        }

        public void SaveRecord(TaskRunningRecord record)
        {
            m_logs.Add(record);
        }

        public T RunFunc<T>(TimeSpan delayDuration, Func<T> runAction, string name = null)
        {
            var recordBuilder = new RecordBuilder().Init(
                Thread.CurrentThread.ManagedThreadId, 
                name);
            try
            {
                Thread.Sleep(delayDuration);
                T result = runAction();
                SaveRecord(recordBuilder.Build(result));
                return result;
            }
            catch (Exception error)
            {
                SaveRecord(recordBuilder.BuildError(error));
                throw;
            }
        }

        public void RunAction(TimeSpan delayDuration, Action runAction, string name = null)
        {
            RunFunc(delayDuration, () =>
            {
                runAction();
                return string.Empty;
            }, name);
        }

        public ITaskVisualizerReport GetReport()
        {
            TaskRunningRecord[] logs = m_logs.ToArray();
            DateTime overallStartTime = logs.Min(log => log.StartTime);
            DateTime overallEndTime = logs.Max(log => log.EndTime);
            TaskVisualizerRecord[] records = logs.Select(log => new TaskVisualizerRecord
            {
                ThreadId = log.ThreadId,
                Duration = (log.EndTime - log.StartTime).TotalSeconds,
                Name = log.Name,
                RelativeEndSecond = (log.EndTime - overallStartTime).TotalSeconds,
                RelativeStartSecond = (log.StartTime - overallStartTime).TotalSeconds,
                TaskError = log.CapturedError.FormatTaskError(),
                TaskResult = log.TaskResult.FormatTaskResult(),
                TaskStatus = (int) log.Status
            }).ToArray();

            ITaskVisualizerThread[] threads = records
                .GroupBy(r => r.ThreadId)
                .Select(
                    g =>
                        (ITaskVisualizerThread) new TaskVisualizerThread
                        {
                            Name = string.Format("Thread #{0}", g.Key),
                            Tasks = g.Cast<ITaskVisualizerRecord>().ToArray()
                        })
                .ToArray();

            return new TaskVisualizerReport
            {
                Threads = threads,
                TotalSeconds = (overallEndTime - overallStartTime).TotalSeconds,
                TotalThreads = threads.Length
            };
        }
    }
}