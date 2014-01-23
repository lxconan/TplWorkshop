using System;

namespace TplWorkshop.Util
{
    public static class TaskVisualizerActionExtension
    {
        public static string Wait1SecAndReturnHello(
            this ITaskVisualizer visualizer, string name = null)
        {
            return visualizer.RunFunc(TimeSpan.FromSeconds(1), () => "hello", name);
        }

        public static string Wait2SecAndReturnHello(
            this ITaskVisualizer visualizer, string name = null)
        {
            return visualizer.RunFunc(TimeSpan.FromSeconds(2), () => "hello", name);
        }

        public static string Wait3SecAndReturnHello(
            this ITaskVisualizer visualizer, string name = null)
        {
            return visualizer.RunFunc(TimeSpan.FromSeconds(3), () => "hello", name);
        }

        public static object Wait1SecAndEcho(
            this ITaskVisualizer visualizer, object state, string name = null)
        {
            return visualizer.RunFunc(TimeSpan.FromSeconds(1), () => state, name);
        }
    }
}