using System;

namespace TplWorkshop.Util.Visualizer
{
    internal class TaskRunningRecord
    {
        private readonly string m_name;
        private readonly Guid m_id;
        private readonly DateTime m_startTime;
        private readonly DateTime m_endTime;
        private readonly int m_threadId;

        public TaskRunningRecord(
            string name,
            Guid id,
            DateTime startTime, 
            DateTime endTime, 
            int threadId)
        {
            m_name = name;
            m_id = id;
            m_startTime = startTime;
            m_endTime = endTime;
            m_threadId = threadId;
        }

        public Guid Id
        {
            get { return m_id; }
        }

        public DateTime StartTime
        {
            get { return m_startTime; }
        }

        public DateTime EndTime
        {
            get { return m_endTime; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public int ThreadId
        {
            get { return m_threadId; }
        }
    }
}