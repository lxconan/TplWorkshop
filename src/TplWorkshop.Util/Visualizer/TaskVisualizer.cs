using System;
using System.Collections.Concurrent;
using System.Linq;

namespace TplWorkshop.Util.Visualizer
{
    internal class TaskVisualizer : ITaskVisualizer
    {
        private readonly ConcurrentBag<TaskRunningRecord> m_logs;
        private readonly ConcurrentBag<Hint> m_hints;

        public TaskVisualizer()
        {
            m_logs = new ConcurrentBag<TaskRunningRecord>();
            m_hints = new ConcurrentBag<Hint>();
        }

        public void SaveRecord(object record)
        {
            m_logs.Add((TaskRunningRecord)record);
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
                RelativeStartSecond = (log.StartTime - overallStartTime).TotalSeconds
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

            ITaskVisualizerHint[] hints = m_hints.Select(
                h =>
                    new TaskVisualizerHint
                    {
                        Description = h.Description,
                        RelativePosition = (h.Position - overallStartTime).TotalSeconds
                    })
                .Cast<ITaskVisualizerHint>()
                .ToArray();

            return new TaskVisualizerReport
            {
                Threads = threads,
                TotalSeconds = (overallEndTime - overallStartTime).TotalSeconds,
                TotalThreads = threads.Length,
                Hints = hints
            };
        }

        public Tracer Start(string name = null)
        {
            return new Tracer(this, name);
        }

        public void Hint(string description)
        {
            m_hints.Add(new Hint {Position = DateTime.Now, Description = description});
        }
    }
}