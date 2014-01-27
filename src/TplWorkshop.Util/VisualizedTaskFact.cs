using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TplWorkshop.Util.EmbeddedResource;
using TplWorkshop.Util.Visualizer;

namespace TplWorkshop.Util
{
    public class VisualizedTaskFact : IDisposable
    {
        private readonly ITaskVisualizer m_visualizer;

        private readonly string m_testCacheFolder;

        protected ITaskVisualizer V { get { return m_visualizer; } }

        public VisualizedTaskFact(ITaskVisualizer visualizer)
        {
            m_visualizer = visualizer;
            m_testCacheFolder = GetTestCacheFolder();
        }

        private static string GetTestCacheFolder()
        {
            var assemblyFolder = AppDomain.CurrentDomain.BaseDirectory;
            var binFolder = Directory.GetParent(assemblyFolder);
            var projectFolder = binFolder.Parent;
            var solutionFolder = projectFolder.Parent;
            var testCacheFolder = Path.Combine(solutionFolder.FullName, "test_cache");
            return testCacheFolder;
        }

        public VisualizedTaskFact() : this(new TaskVisualizer())
        {
        }

        public void Dispose()
        {
            ITaskVisualizerReport taskVisualizerReport = m_visualizer.GetReport();
            string serializeObject = JsonConvert.SerializeObject(
                taskVisualizerReport,
                Formatting.None);
            string report = Resource.test_template.Replace(
                "%REPORT_DATA_PLACEHOLDER%", serializeObject);
            string resultFile = CreateReportFile(report);

            Trace.WriteLine("Please click on the following file to see the task executing chart.");
            Trace.WriteLine("file://" + resultFile.Replace("\\", "/"));
        }

        private string CreateReportFile(string report)
        {
            string path = Path.Combine(
                m_testCacheFolder, "r_" + Guid.NewGuid().ToString("N") + ".htm");
            File.WriteAllText(path, report, Encoding.UTF8);
            return path;
        }
    }
}