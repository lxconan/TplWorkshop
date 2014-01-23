using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace TplWorkshop.Util.Visualizer
{
    internal class TaskVisualizer : ITaskVisualizer
    {
        private readonly ConcurrentDictionary<int, ConcurrentQueue<TaskRunningRecord>> m_logs =
            new ConcurrentDictionary<int, ConcurrentQueue<TaskRunningRecord>>();

        public T RunAction<T>(TimeSpan delayDuration, Func<T> runAction, string name = null)
        {
            RegisterCurrentThread();
            var recordBuilder = new RecordBuilder().Init(name);
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

        private void SaveRecord(TaskRunningRecord record)
        {
            m_logs[Thread.CurrentThread.ManagedThreadId].Enqueue(record);
        }

        private void RegisterCurrentThread()
        {
            m_logs.TryAdd(Thread.CurrentThread.ManagedThreadId, new ConcurrentQueue<TaskRunningRecord>());
        }

        public ITaskVisualizerReport GetReport()
        {
            var report = new TaskVisualizerReport();
            foreach (KeyValuePair<int, ConcurrentQueue<TaskRunningRecord>> threadLog in m_logs)
            {
                report.AddThreadRecords(threadLog.Key, threadLog.Value);
            }

            return report;
        }
    }
}