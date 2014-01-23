namespace TplWorkshop.Util.Visualizer
{
    internal class TaskVisualizerReport : ITaskVisualizerReport
    {
        public double TotalSeconds { get; set; }
        public int TotalThreads { get; set; }
        public ITaskVisualizerThread[] Threads { get; set; }
    }
}