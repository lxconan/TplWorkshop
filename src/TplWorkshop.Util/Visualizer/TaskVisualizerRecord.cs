namespace TplWorkshop.Util.Visualizer
{
    internal class TaskVisualizerRecord : ITaskVisualizerRecord
    {
        public string Name { get; set; }
        public double RelativeStartSecond { get; set; }
        public double RelativeEndSecond { get; set; }
        public double Duration { get; set; }
        public string TaskResult { get; set; }
        public string TaskError { get; set; }
        public int TaskStatus { get; set; }
        public int ThreadId { get; set; }
    }
}