using System;
using System.Threading.Tasks;

namespace TplWorkshop.Util.Visualizer
{
    internal class TaskRunningRecord
    {
        private readonly string m_name;
        private readonly Guid m_id;
        private readonly DateTime m_startTime;
        private readonly DateTime m_endTime;
        private readonly object m_taskResult;
        private readonly Exception m_capturedError;
        private readonly int m_threadId;

        public TaskRunningRecord(
            string name,
            Guid id,
            DateTime startTime, 
            DateTime endTime,
            object taskResult, 
            Exception capturedError, 
            int threadId)
        {
            m_name = name;
            m_id = id;
            m_startTime = startTime;
            m_endTime = endTime;
            m_taskResult = taskResult;
            m_capturedError = capturedError;
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

        public object TaskResult
        {
            get { return m_taskResult; }
        }

        public Exception CapturedError
        {
            get { return m_capturedError; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public TaskStatus Status
        {
            get
            {
                if (m_capturedError == null)
                {
                    return TaskStatus.RanToCompletion;
                }

                if (m_capturedError is TaskCanceledException)
                {
                    return TaskStatus.Canceled;
                }
                    
                return TaskStatus.Faulted;
            }
        }

        public int ThreadId
        {
            get { return m_threadId; }
        }
    }
}