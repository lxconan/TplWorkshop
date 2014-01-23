using System;
using System.Threading;

namespace TplWorkshop.Util.Visualizer
{
    internal class RecordBuilder
    {
        private static int s_untitledRef;
        private DateTime? m_startTime;
        private DateTime? m_endTime;
        private object m_result;
        private Exception m_error;
        private string m_name;
        private bool m_initialized;
        private int m_threadId;

        public RecordBuilder Init(int threadId, string name)
        {
            if (m_initialized)
            {
                throw new InvalidOperationException("Cannot initialize twice.");
            }

            m_threadId = threadId;
            m_startTime = DateTime.Now;
            var normalizedName = string.IsNullOrWhiteSpace(name) 
                ? GenerateUntitled() 
                : name;
            m_name = normalizedName;
            m_initialized = true;
            return this;
        }

        private static string GenerateUntitled()
        {
            int titleId = Interlocked.Increment(ref s_untitledRef);
            return "Untitled " + titleId;
        }

        internal static void ResetTitleRef()
        {
            Interlocked.Exchange(ref s_untitledRef, 0);
        }

        public TaskRunningRecord Build(object result)
        {
            return Build(result, false);
        }

        public TaskRunningRecord BuildError(Exception result)
        {
            return Build(result, true);
        }

        private TaskRunningRecord Build(object result, bool isError)
        {
            SaveResult(result, isError);
            m_endTime = DateTime.Now;
            return new TaskRunningRecord(
                m_name,
                Guid.NewGuid(),
                GetStartTime(),
                GetEndTime(),
                m_result,
                m_error,
                m_threadId);
        }

        private void SaveResult(object result, bool isError)
        {
            if (isError)
            {
                m_error = (Exception)result;
            }
            else
            {
                m_result = result;
            }
        }

        private DateTime GetEndTime()
        {
            if (!m_endTime.HasValue)
            {
                throw new InvalidOperationException("End time missing.");
            }

            return m_endTime.Value;
        }

        private DateTime GetStartTime()
        {
            if (!m_startTime.HasValue)
            {
                throw new InvalidOperationException("Start time missing.");
            }

            return m_startTime.Value;
        }
    }
}