﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TplWorkshop.Util.Visualizer;
using Xunit;

namespace TplWorkshop.Util.Facts
{
    public class TaskVisualizerFact
    {
        private static List<ITaskVisualizerRecord> GetAllRecords(ITaskVisualizerReport report)
        {
            return report.Threads.SelectMany(r => r.Tasks).ToList();
        }
            
        [Fact]
        public void should_add_new_record_after_action_running()
        {
            var taskVisualizer = new TaskVisualizer();
            taskVisualizer.Start().Sleep(1).Dispose();
            ITaskVisualizerReport report = taskVisualizer.GetReport();
            Assert.Equal(1, GetAllRecords(report).Count);
        }

        [Fact]
        public void should_record_name()
        {
            var taskVisualizer = new TaskVisualizer();
            const string expected = "task";
            taskVisualizer.Start(expected).Dispose();

            ITaskVisualizerReport report = taskVisualizer.GetReport();
            ITaskVisualizerRecord record = GetAllRecords(report).Single();

            Assert.Equal(expected, record.Name);
        }

        [Fact]
        public void should_delay_and_execute()
        {
            var taskVisualizer = new TaskVisualizer();
            const int delayDuration = 3;
            taskVisualizer.Start().Sleep(delayDuration).Dispose();

            ITaskVisualizerReport report = taskVisualizer.GetReport();
            ITaskVisualizerRecord record = GetAllRecords(report).Single();

            Assert.True(record.Duration >= delayDuration);
        }
    }
}