using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boeing.Algorithms.Scheduling.Core;
using Boeing.Algorithms.Scheduling.Model;
using Boeing.Algorithms.Scheduling.Model.Jobs;
using Boeing.Algorithms.Scheduling.Model.Resources.Tools;
using Boeing.Algorithms.Scheduling.Model.Resources.Workers;
using Boeing.Algorithms.Scheduling.Model.Resources.Zones;
using Boeing.Algorithms.Scheduling.Model.Shifts;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Analytics.ConstraintSolver.Solvers;
using DataSetServices.Data.Analytics.ConstraintSolver.Utilities;
using DataSetServices.Data.Core.BaseTypes;
using DataSetServices.Data.Domain;
using DataSetServices.Data.Modeling.Core;
using DataSetServices.Data.Modeling.Projects;
using DataSetServices.Data.Modeling.Resources;
using QuickGraph;
using QuickGraph.Algorithms.MaximumFlow;
using IShift = Boeing.Algorithms.Scheduling.Model.Shifts.IShift;
using ITool = Boeing.Algorithms.Scheduling.Model.Resources.Tools.ITool;
using IZone = Boeing.Algorithms.Scheduling.Model.Resources.Zones.IZone;
using ShiftFactory = Boeing.Algorithms.Scheduling.Model.Shifts.ShiftFactory;
using ToolFactory = Boeing.Algorithms.Scheduling.Model.Resources.Tools.ToolFactory;
using ZoneFactory = Boeing.Algorithms.Scheduling.Model.Resources.Zones.ZoneFactory;

namespace Boeing.Algorithms.Solvers.Utils
{
    public class DataSetSchedulingUtility : IDisposable
    {
        #region Constructor

        public DataSetSchedulingUtility()
        {
            InitializeShifts();
            InitializeSkills();
            InitializeWorkers();
            InitializeTools();
            InitializeZones();
            InitializeJobs();
        }

        #endregion END Constructor

        #region Private Fields

        #region Shifts

        private ShiftFactory _shiftFactory;
        private ShiftConverter _shiftConverter;
        private ConcurrentDictionary<string, IShift> _shifts;
        private ConcurrentDictionary<int, DataSetServices.Data.Modeling.Core.IShift> _shiftsMap;

        private void InitializeShifts()
        {
            _shifts = new ConcurrentDictionary<string, IShift>();
            _shiftFactory = new ShiftFactory();
            _shiftConverter = new ShiftConverter(_shiftFactory);
            _shifts = new ConcurrentDictionary<string, IShift>();
            _shiftsMap = new ConcurrentDictionary<int, DataSetServices.Data.Modeling.Core.IShift>();
        }

        #endregion END Shifts

        #region Skills

        private ConcurrentDictionary<string, ISkill> _skills;
        private SkillFactory _skillFactory;
        private SkillConverter _skillConverter;

        private void InitializeSkills()
        {
            _skills = new ConcurrentDictionary<string, ISkill>();
            _skillFactory = new SkillFactory();
            _skillConverter = new SkillConverter(_skillFactory);
        }

        #endregion END Skills

        #region Workers

        private WorkerFactory _workerFactory;
        private LaborConverter _laborConverter;
        private ConcurrentDictionary<int, IWorker> _workers;
        private ConcurrentDictionary<int, ILabor> _laborsMap;

        private void InitializeWorkers()
        {
            _workerFactory = new WorkerFactory();
            _laborConverter = new LaborConverter(_workerFactory, _shiftConverter, _skillConverter);
            _workers = new ConcurrentDictionary<int, IWorker>();
            _laborsMap = new ConcurrentDictionary<int, ILabor>();
        }

        #endregion END Workers

        #region Tools

        private ToolFactory _toolFactory;
        private ToolConverter _toolConverter;
        private ConcurrentDictionary<int, ITool> _tools;
        private ConcurrentDictionary<string, ITool> _toolTypes;
        private ConcurrentDictionary<int, DataSetServices.Data.Modeling.Resources.ITool> _toolsMap;

        private void InitializeTools()
        {
            _toolFactory = new ToolFactory();
            _toolConverter = new ToolConverter(_toolFactory, _shiftConverter);
            _tools = new ConcurrentDictionary<int, ITool>();
            _toolTypes = new ConcurrentDictionary<string, ITool>();
            _toolsMap = new ConcurrentDictionary<int, DataSetServices.Data.Modeling.Resources.ITool>();
        }

        #endregion END Tools

        #region Zones

        private ZoneFactory _zoneFactory;
        private ZoneConverter _zoneConverter;
        private ConcurrentDictionary<int, IZone> _zones;
        private ConcurrentDictionary<int, DataSetServices.Data.Modeling.Resources.IZone> _zonesMap;

        private void InitializeZones()
        {
            _zoneFactory = new ZoneFactory();
            _zoneConverter = new ZoneConverter(_zoneFactory);
            _zones = new ConcurrentDictionary<int, IZone>();
            _zonesMap = new ConcurrentDictionary<int, DataSetServices.Data.Modeling.Resources.IZone>();
        }

        #endregion END Zones

        #region Jobs

        private JobFactory _jobFactory;
        private JobConverter _jobConverter;
        private ConcurrentDictionary<int, IJob> _jobs;
        private ConcurrentDictionary<int, IActivity> _activitiesMap;
        private ConcurrentDictionary<Guid, IActivitySchedule> _scheduledJobs;

        private void InitializeJobs()
        {
            _jobFactory = new JobFactory();
            _jobConverter = new JobConverter(_jobFactory, _skillConverter, _toolConverter, _zoneConverter);
            _jobs = new ConcurrentDictionary<int, IJob>();
            _activitiesMap = new ConcurrentDictionary<int, IActivity>();
            _scheduledJobs = new ConcurrentDictionary<Guid, IActivitySchedule>();
        }

        #endregion END Jobs

        #endregion END Private Fields

        #region Public Solver Code

        public async Task<ConstraintSolverResult> ScheduleDataSetAsync(ProjectDataSet dataSet, IConstraints constraints, GreedyActivityStrategy strategy)
        {
            return await Task.Run(() => ScheduleDataSet(dataSet, constraints, strategy));
        }

        public ConstraintSolverResult ScheduleDataSet(ProjectDataSet dataSet, IConstraints constraints, GreedyActivityStrategy strategy)
        {
            var result = new ConstraintSolverResult(constraints);

            try
            {
                var cp = new CriticalPathHelper();
                cp.OrderActivitySchedule(dataSet, result);
                foreach (var a in result.Schedule)
                {
                    _scheduledJobs.GetOrAdd(a.ScheduledItem.UID, a);
                }

                SetupJobsAndResources(dataSet, constraints);
                var dataManager = new SolverDataManager(dataSet, result);
                var solver = new Solver(dataManager);

                //var results = solver.Solve(
                //    _shifts.Values.ToList(),
                //    _workers.Values.ToList(),
                //    _tools.Values.ToList(),
                //    _zones.Values,
                //    _jobs.Values,
                //    strategy, _shiftManager);

                //foreach (var job in results)
                //{
                //    var activity = _activitiesMap[job.Job.ID];
                //    var scheduledActivity = _scheduledJobs[activity.UID];

                //    var start = TimeFrame.FromMinutes(job.Start);
                //    var end = TimeFrame.FromMinutes(job.End);

                //    scheduledActivity.ScheduledStart = start;
                //    scheduledActivity.ScheduledFinish = end;

                //    //result.ActivitySchedule.AddOrUpdateItemSchedule(job.Start, scheduledActivity);
                //    //result.State = SolverResultState.OptimalSolutionFound;
                //}
                //result.State = SolverResultState.OptimalSolutionFound;
            }
            catch (Exception ex)
            {
                throw new Exception("Yikes! Error in solver!", ex);
            }

            return result;
        }

        #endregion END Public Solver Code

        #region Graph Code

        private EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>> _flowAgorithm;
        private IMutableVertexAndEdgeListGraph<int, Edge<int>> _graph;

        private void InitializeGraph()
        {
            GenerateActivityGraph();

            _flowAgorithm =
                new EdmondsKarpMaximumFlowAlgorithm<int, Edge<int>>(
                    _graph,
                    CalculateActivitySuccessorCapacity,
                    CreateAdditionalEdgeFactory);
        }

        private void GenerateActivityGraph()
        {
            _graph = new AdjacencyGraph<int, Edge<int>>();
        }

        private Edge<int> CreateAdditionalEdgeFactory(int source, int target)
        {
            return new Edge<int>(source, target);
        }

        private double CalculateActivitySuccessorCapacity(Edge<int> edge)
        {
            return 0d;
        }

        #endregion END Graph Code

        #region Data Structures Setup Logic

        private ShiftManager _shiftManager;

        private void SetupJobsAndResources(ProjectDataSet dataSet, IConstraints constraints)
        {

            //foreach (var shift in dataSet.Shifts)
            //{
            //    var cshift = CreateShiftFromShift(shift);
            //    _shiftManager.AddShift(cshift);
            //    _shifts.GetOrAdd(shift.Name, cshift);

            //    if (constraints.ConstrainOnLaborPool)
            //    {
            //        foreach (var laborLink in shift.LaborLinks)
            //        {
            //            foreach (var w in CreateWorkerFromLaborLink(laborLink))
            //            {
            //                _workers.GetOrAdd(w.ID, w);
            //                _shifts[shift.Name].Workers.Add(w);
            //            }
            //        }
            //    }

            //    if (constraints.ConstrainOnTools)
            //    {
            //        foreach (var toolLink in shift.ToolLinks)
            //        {
            //            foreach (var t in CreateToolFromToolLink(toolLink))
            //            {
            //                _tools.GetOrAdd(t.ID, t);
            //                _shifts[shift.Name].Tools.Add(t);
            //            }
            //        }
            //    }
            //}

            if (constraints.ConstrainOnZones)
            {
                foreach (var zone in dataSet.GetZoneResources())
                {
                    var z = CreateZoneFromZone(zone);
                    _zones.GetOrAdd(z.ID, z);
                    _zonesMap.GetOrAdd(z.ID, zone);
                }
            }

            
        }

        private IShift CreateShiftFromShift(DataSetServices.Data.Modeling.Core.IShift shift)
        {
            return _shiftConverter.Convert(shift);
        }

        private IEnumerable<IWorker> CreateWorkerFromLaborLink(AvailableLaborLink laborLink)
        {
            return _laborConverter.Convert(laborLink);
        }

        private IEnumerable<ITool> CreateToolFromToolLink(AvailableToolLink toolLink)
        {
            return _toolConverter.Convert(toolLink);
        }

        private IZone CreateZoneFromZone(DataSetServices.Data.Modeling.Resources.IZone zone)
        {
            return _zoneFactory.Create(zone.Name, zone.MaxOccupants);
        }

        private void CreateSuccessorLink(IJob a, IJob b)
        {
            a.Successors.Add(b);
            if (!b.Predecessors.Contains(a))
            {
                b.Predecessors.Add(a);
            }
        }

        public void Dispose()
        {
        }

        #endregion END Data Structures Setup Logic
    }
}
