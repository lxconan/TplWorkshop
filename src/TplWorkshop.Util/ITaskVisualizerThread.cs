namespace TplWorkshop.Util
{
    public interface ITaskVisualizerThread
    {
        string Name { get; }
        ITaskVisualizerRecord[] Tasks { get; }
    }
}