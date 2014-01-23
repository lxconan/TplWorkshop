using System;

namespace TplWorkshop.Util
{
    public interface ITaskVisualizer
    {
        T RunFunc<T>(TimeSpan delayDuration, Func<T> runAction, string name = null);
        void RunAction(TimeSpan delayDuration, Action runAction, string name = null);
        ITaskVisualizerReport GetReport();
    }
}