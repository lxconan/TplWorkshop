namespace TplWorkshop.Util.Visualizer
{
    internal class TaskVisualizerThread : ITaskVisualizerThread
    {
        public string Name { get; set; }
        public ITaskVisualizerRecord[] Tasks { get; set; }
    }
}