using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boeing.Algorithms.Scheduling.Core;
using Boeing.Algorithms.Scheduling.Model.Jobs;
using Boeing.Algorithms.Scheduling.Model.Resources.Tools;
using Boeing.Algorithms.Scheduling.Model.Resources.Workers;
using Boeing.Algorithms.Scheduling.Model.Resources.Zones;
using Boeing.Algorithms.Scheduling.Model.Shifts;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Analytics.ConstraintSolver.Solvers;
using DataSetServices.Data.Analytics.ConstraintSolver.Utilities;
using DataSetServices.Data.Core.BaseTypes;
using DataSetServices.Data.Modeling.Core;
using DataSetServices.Data.Modeling.Projects;

namespace Boeing.Algorithms.Scheduling.Model
{
    public class SolverDataManager : ISolverDataManager
    {
        #region Constructor

        public SolverDataManager(ProjectDataSet model, ConstraintSolverResult data)
        {
            _model = model;
            _solverResult = data;

            var jobFactory = new JobFactory();
            var skillFactory = new SkillFactory();
            var workerFactory = new WorkerFactory();
            var toolFactory = new ToolFactory();
            var zoneFactory = new ZoneFactory();
            var shiftFactory = new ShiftFactory();
            
            var shiftConverter = new ShiftConverter(shiftFactory);
            var skillConverter = new SkillConverter(skillFactory);
            var toolConverter = new ToolConverter(toolFactory, shiftConverter);
            var zoneConverter = new ZoneConverter(zoneFactory);
            var laborConverter = new LaborConverter(workerFactory, shiftConverter, skillConverter);
            var jobConverter = new JobConverter(jobFactory, skillConverter, toolConverter, zoneConverter);

            _shiftManager = new ShiftManager(shiftConverter, skillConverter, laborConverter, toolConverter);
            _jobManager = new JobManager(jobConverter);
            _zoneManager = new ZoneManager(zoneConverter);
        }

        #endregion END Constructor

        #region Private Fields

        private readonly ProjectDataSet _model;
        private readonly ConstraintSolverResult _solverResult;

        private readonly ShiftManager _shiftManager;
        private readonly JobManager _jobManager;
        private readonly ZoneManager _zoneManager;

        private ConcurrentDictionary<int, IActivity> _activitiesMap;
        private ConcurrentDictionary<Guid, IActivitySchedule> _scheduledJobs;

        #endregion END Private Fields

        #region ISolverDataManager Logic

        public ShiftManager ShiftManager { get { return _shiftManager; } }
        public JobManager JobManager { get { return _jobManager; } }
        public ZoneManager ZoneManager { get { return _zoneManager; } }

        public async Task<ConstraintSolverResult> InitializeDataAsync()
        {
            _activitiesMap = new ConcurrentDictionary<int, IActivity>();
            _scheduledJobs = new ConcurrentDictionary<Guid, IActivitySchedule>();

            await Task.Run(() =>
            {
                var cp = new CriticalPathHelper();
                cp.OrderActivitySchedule(_model, _solverResult);

                foreach (var scheduledItem in _solverResult.Schedule)
                {
                    var activity = scheduledItem.ScheduledItem;
                    _scheduledJobs.GetOrAdd(activity.UID, scheduledItem);
                }
            });

            ShiftManager.ProcessModel(_model, _solverResult.Constraints);
            JobManager.ProcessModelActivities(_solverResult.Schedule);
            ZoneManager.ProcessZones(_model.GetZoneResources());

            return _solverResult;
        }

        public async Task<SolverResultState> ProcessSolution(IEnumerable<IScheduledJob> scheduledJobs)
        {
            await Task.Run(() =>
            {
                // clear out any previously generated schedule
                _solverResult.ActivitySchedule.ItemSchedules.Clear();

                foreach (var job in scheduledJobs)
                {
                    var scheduledActivity = _scheduledJobs[job.Job.SchedulableItemUID];

                    var start = TimeFrame.FromMinutes(job.Start);
                    var end = TimeFrame.FromMinutes(job.End);

                    scheduledActivity.ScheduledStart = start;
                    scheduledActivity.ScheduledFinish = end;

                    _solverResult.ActivitySchedule.AddOrUpdateItemSchedule(job.Start, scheduledActivity);
                }
            });

            return SolverResultState.OptimalSolutionFound;
        }

        #endregion END ISolverDataManager Logic
    }
}
