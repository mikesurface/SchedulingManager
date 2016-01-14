using System;
using System.Collections.Generic;
using Boeing.Algorithms.Scheduling.Core;
using Boeing.Algorithms.Scheduling.Model.Jobs;
using Boeing.Algorithms.Scheduling.Model.Shifts;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Tools
{
    public class ToolManager
    {
        public ToolManager(IShift shift, ToolConverter converter)
        {
            _shift = shift;
            _converter = converter;
            _inProcessJobs = new Dictionary<IToolInstance, IJob>();
            _inProcessTools = new Dictionary<IJob, HashSet<IToolInstance>>();
            _inProcessToolSchedules = new Dictionary<IJob, HashSet<IScheduledTool>>();
            _processedJobs = new Dictionary<IJob, Tuple<int, int>>();
            _toolAvailableStates = new Dictionary<ITool, Dictionary<int, bool>>();
            foreach (var tool in _converter.ToolTypes)
            {
                _toolAvailableStates.Add(tool.Key, new Dictionary<int, bool>());
                foreach (var ti in tool.Value)
                {
                    _toolAvailableStates[tool.Key].Add(ti.Key, true);
                }
            }
        }
        
        private readonly IShift _shift;
        private readonly ToolConverter _converter;

        private readonly Dictionary<IToolInstance, IJob> _inProcessJobs;
        private readonly Dictionary<IJob, HashSet<IToolInstance>> _inProcessTools;
        private readonly Dictionary<IJob, HashSet<IScheduledTool>> _inProcessToolSchedules;
        private readonly Dictionary<IJob, Tuple<int, int>> _processedJobs;

        private readonly Dictionary<ITool, Dictionary<int, bool>> _toolAvailableStates;

        public IEnumerable<ITool> GetTools()
        {
            return _converter.Instances.Values;
        }
        
        private IToolInstance GetNextTool(ITool tool)
        {
            foreach (var toolInstanceState in _toolAvailableStates[tool])
            {
                if (toolInstanceState.Value)
                {
                    // return first worker that is available
                    return _converter.ToolTypes[tool][toolInstanceState.Key];
                }
            }
            return null;
        }

        public bool ScheduleJob(IJob job)
        {
            if (_inProcessTools.ContainsKey(job)) return false;

            var tools = new HashSet<IToolInstance>();
            _inProcessTools.Add(job, tools);
            var toolSchedules = new HashSet<IScheduledTool>();
            _inProcessToolSchedules.Add(job, toolSchedules);

            foreach (var tool in job.RequiredToolSet)
            {
                var next = GetNextTool(tool);
                if (next == null) return false;
                tools.Add(next);
                var scheduledTool = new ScheduledTool(next, job.ScheduledStartTime, job.ScheduledEndTime, job);
                toolSchedules.Add(scheduledTool);
                _inProcessJobs.Add(next, job);
                _toolAvailableStates[tool][next.ID] = false;
            }
            return true;
        }

        public void CompleteJob(IJob job)
        {
            if (_inProcessTools.ContainsKey(job))
            {
                foreach (var toolInstance in _inProcessTools[job])
                {
                    _inProcessJobs.Remove(toolInstance);
                    _toolAvailableStates[toolInstance.Definition][toolInstance.ID] = true;
                }
                if (!_processedJobs.ContainsKey(job))
                {
                    _processedJobs.Add(job, new Tuple<int, int>(job.ScheduledStartTime, job.ScheduledEndTime));
                }
                _inProcessTools.Remove(job);
            }
        }

        public bool CanProcess(IJob job)
        {
            foreach (var tool in job.RequiredToolSet)
            {
                var next = GetNextTool(tool);
                if (next == null) return false;
            }
            return true;
        }
    }
}