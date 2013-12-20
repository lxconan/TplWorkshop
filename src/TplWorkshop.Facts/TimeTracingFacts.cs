using System;
using System.Diagnostics;
using System.Threading;

namespace TplWorkshop.Facts
{
    public class TimeTracingFacts : IDisposable
    {
        private readonly Stopwatch m_stopwatch;

        public TimeTracingFacts()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            Trace.WriteLine(string.Format("Unit test is running on thread {0}", threadId));

            m_stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            m_stopwatch.Stop();
            Trace.WriteLine("Total elapsed time: " + m_stopwatch.Elapsed);
        }
    }
}