using System;

namespace TplWorkshop.Util
{
    public interface ITaskVisualizer
    {
        T RunAction<T>(TimeSpan delayDuration, Func<T> runAction, string name = null);
        ITaskVisualizerReport GetReport();
    }
}