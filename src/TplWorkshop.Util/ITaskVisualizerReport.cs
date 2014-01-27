namespace TplWorkshop.Util
{
    public interface ITaskVisualizerReport
    {
        double TotalSeconds { get; }
        int TotalThreads { get; }
        ITaskVisualizerThread[] Threads { get; }
        ITaskVisualizerHint[] Hints { get; }
    }
}