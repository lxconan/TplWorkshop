using System;
using System.Diagnostics;
using Newtonsoft.Json;
using TplWorkshop.Util.Visualizer;

namespace TplWorkshop.Util
{
    public class VisualizedTaskFact : IDisposable
    {
        private readonly ITaskVisualizer m_visualizer;

        protected ITaskVisualizer Visualizer { get { return m_visualizer; } }

        public VisualizedTaskFact(ITaskVisualizer visualizer)
        {
            m_visualizer = visualizer;
        }

        public VisualizedTaskFact() : this(new TaskVisualizer())
        {
        }

        public void Dispose()
        {
            ITaskVisualizerReport taskVisualizerReport = m_visualizer.GetReport();
            string serializeObject = JsonConvert.SerializeObject(
                taskVisualizerReport,
                Formatting.Indented);
            Trace.WriteLine(serializeObject);
        }
    }
}