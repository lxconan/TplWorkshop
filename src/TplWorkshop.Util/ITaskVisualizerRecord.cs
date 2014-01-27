namespace TplWorkshop.Util
{
    public interface ITaskVisualizerRecord
    {
        string Name { get; }
        double RelativeStartSecond { get; }
        double RelativeEndSecond { get; }
        double Duration { get; }
        int ThreadId { get; }
    }
}