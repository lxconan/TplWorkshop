namespace TplWorkshop.Util
{
    public interface ITaskVisualizer
    {
        ITaskVisualizerReport GetReport();
        void SaveRecord(object record);
        Tracer Start(string name = null);
        void Hint(string description);
    }
}