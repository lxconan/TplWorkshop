namespace TplWorkshop.Util
{
    public interface ITaskVisualizerRecord
    {
        string Name { get; }
        double RelativeStartSecond { get; }
        double RelativeEndSecond { get; }
        double Duration { get; }
        string TaskResult { get; }
        string TaskError { get; }
        int TaskStatus { get; }
        int ThreadId { get; }
    }
}