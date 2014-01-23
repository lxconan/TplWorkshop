using System;
using TplWorkshop.Util.Visualizer;

namespace TplWorkshop.Util
{
    public class VisualizedTaskFact : IDisposable
    {
        private readonly ITaskVisualizer m_visualizer;

        public VisualizedTaskFact(ITaskVisualizer visualizer)
        {
            m_visualizer = visualizer ?? new TaskVisualizer();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}