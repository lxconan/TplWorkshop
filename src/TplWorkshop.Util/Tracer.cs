using System;
using System.Threading;
using TplWorkshop.Util.Visualizer;

namespace TplWorkshop.Util
{
    public class Tracer : IDisposable
    {
        private readonly ITaskVisualizer m_visualizer;
        private readonly RecordBuilder m_recordBuilder;
        private bool m_disposed;

        public Tracer(ITaskVisualizer visualizer, string name)
        {
            m_visualizer = visualizer;
            m_recordBuilder = new RecordBuilder().Init(
                Thread.CurrentThread.ManagedThreadId,
                name);
        }

        public Tracer Sleep(int seconds)
        {
            Thread.Sleep(seconds * 1000);
            return this;
        }

        public void Dispose()
        {
            if (m_disposed) return;

            m_visualizer.SaveRecord(m_recordBuilder.Build());
            m_disposed = true;
        }

        public void Finish()
        {
            Dispose();
        }

        public void FinishError(Exception error)
        {
            Dispose();
            throw error;
        }

        public T Finish<T>(T value)
        {
            Dispose();
            return value;
        }
    }
}