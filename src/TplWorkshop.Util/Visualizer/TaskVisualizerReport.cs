using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TplWorkshop.Util.Visualizer
{
    internal class TaskVisualizerReport : ITaskVisualizerReport
    {
        private readonly Dictionary<int, List<TaskRunningRecord>> m_logs = 
            new Dictionary<int, List<TaskRunningRecord>>();

        public void AddThreadRecords(int threadId, IEnumerable<TaskRunningRecord> records)
        {
            m_logs.Add(threadId, new List<TaskRunningRecord>(records));
        }

        public IReadOnlyList<TaskRunningRecord> GetAllRecords()
        {
            return m_logs.SelectMany(item => item.Value).ToList().AsReadOnly();
        }

        public IReadOnlyDictionary<int, List<TaskRunningRecord>> Data
        {
            get { return new ReadOnlyDictionary<int, List<TaskRunningRecord>>(m_logs); }
        }

        public DateTime GetFirstStartTime()
        {
            IReadOnlyList<TaskRunningRecord> records = GetAllRecords();
            return records.Min(r => r.StartTime);
        }

        public DateTime GetLastEndTime()
        {
            IReadOnlyList<TaskRunningRecord> records = GetAllRecords();
            return records.Max(r => r.EndTime);
        }
    }
}