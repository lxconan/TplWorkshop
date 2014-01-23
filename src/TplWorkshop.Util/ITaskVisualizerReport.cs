using System;
using System.Collections.Generic;
using TplWorkshop.Util.Visualizer;

namespace TplWorkshop.Util
{
    public interface ITaskVisualizerReport
    {
        IReadOnlyDictionary<int, List<TaskRunningRecord>> Data { get; }
        DateTime GetFirstStartTime();
        DateTime GetLastEndTime();
    }
}