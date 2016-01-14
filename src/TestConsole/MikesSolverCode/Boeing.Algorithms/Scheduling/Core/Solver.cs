using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Boeing.Algorithms.Scheduling.Model;
using Boeing.Algorithms.Scheduling.Model.Jobs;
using Boeing.Algorithms.Scheduling.Model.Resources.Tools;
using Boeing.Algorithms.Scheduling.Model.Resources.Workers;
using Boeing.Algorithms.Scheduling.Model.Resources.Zones;
using Boeing.Algorithms.Scheduling.Model.Shifts;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;

namespace Boeing.Algorithms.Scheduling.Core
{
    public interface IMultiThreadedSolver
    {
        Task<SolverResultState> ComputeSolutionAsync();
    }

    public class Solver : IMultiThreadedSolver
    {
        public Solver(SolverDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        private readonly SolverDataManager _dataManager;

        public async Task<SolverResultState> ComputeSolutionAsync()
        {
            SolverResultState resultState = SolverResultState.Running;

            await Task.Run(() =>
            {
                var result = TryGenerateSolution();
                if (!result) resultState = SolverResultState.Error;
            });

            if (resultState != SolverResultState.Running) return resultState;

            var results = BuildSchedule();
            
            return await _dataManager.ProcessSolution(results);
        }

        protected bool TryGenerateSolution()
        {
            _shiftManager = _dataManager.ShiftManager;
            _jobManager = _dataManager.JobManager;
            _zoneManager = _dataManager.ZoneManager;

            _shifts = _shiftManager.GetShifts().ToArray();
            var rootJobs = _jobManager.RootJobs;
            var jobs = _jobManager.GetJobs();

            _jobs = new List<IJob>(jobs);

            _zones = new List<IZone>(_zoneManager.GetZones());

            _jobPriorityFunction = new SlackTimeRemainingJobPriorityFunction();
            _availableJobs = new JobProcessorList(_jobPriorityFunction);

            foreach (var job in rootJobs)
            {
                _availableJobs.Enqueue(job);
            }

            _timepoints = new Dictionary<int, List<InWorkEvent>>();
            _inWorkEvents = new List<InWorkEvent>();

            int iteration = InitializeNextIteration(_shifts[0]);

            while (_jobs.Any() && iteration < MaxIterationCount)
            {
                // check to see if we have any events expiring
                ProcessCompletingJobs(iteration);
                
                // check to see if we have any new jobs we can work on
                iteration = ProcessJobs(iteration);

                // increment iteration counter
                iteration = IncrementIteration(iteration);
            }

            return true;
        }

        private int IncrementIteration(int currentIteration)
        {
            return ++currentIteration;
        }

        #region Solve IT!!!

        private GreedyActivityStrategy _strategy;
        private JobPriorityFunction _jobPriorityFunction;
        private const int MaxIterationCount = 24*60*365*4; // 4 years of minutes
        protected IEnumerable<IScheduledJob> Solve(
            List<IShift> shifts, 
            List<IWorker> workers, 
            List<ITool> tools, 
            IEnumerable<IZone> zones, 
            IEnumerable<IJob> jobs, 
            GreedyActivityStrategy strategy,
            ShiftManager shiftManager)
        {
            _shiftManager = shiftManager;
            _strategy = strategy;
            _jobPriorityFunction = new SlackTimeRemainingJobPriorityFunction();

            if (shifts.Count > 0)
            {
                _shifts = shifts.ToArray();
                SetupWorkers(shifts, workers);
                SetupTools(shifts, tools);
            }

            SetupZones(zones);

            SetupJobs(jobs);

            _timepoints = new Dictionary<int, List<InWorkEvent>>();
            _inWorkEvents = new List<InWorkEvent>();

            int iteration = InitializeNextIteration(_shifts[0]);

            while (_jobs.Any() && iteration < MaxIterationCount)
            {
                // check to see if we have any events expiring
                ProcessCompletingJobs(iteration);
                
                // check to see if we have any new jobs we can work on
                ProcessJobs(iteration);

                // increment iteration counter
                iteration++;
            }

            // we're done!
            return BuildSchedule();
        }

        private int InitializeNextIteration(IShift shift)
        {
            _currentShift = shift;
            return _currentShift.NextStart();
        }

        #endregion END Solve IT!!!

        #region Private Fields

        private IShift[] _shifts;
        private List<IZone> _zones;
        private List<IJob> _jobs;
        private IJobProcessor _availableJobs;
        private Dictionary<int, List<InWorkEvent>> _timepoints;
        private List<InWorkEvent> _inWorkEvents;

        #region Matrix (Bar Chart) Of TimeSlots and InProcessJobs

        private ShiftManager _shiftManager;
        private JobManager _jobManager;
        private ZoneManager _zoneManager;

        // for each minute of the schedule, we maintain a dictionary of workers by shift
        // and the current job (if any) they are working on
        private Dictionary<IShift, Dictionary<int, Dictionary<IWorker, IJob>>> _workerBarChart;
        
        // for each minute of the schedule, we maintain a dictionary of tools by shift
        // and the current job (if any) that its working on
        private Dictionary<IShift, Dictionary<int, Dictionary<ITool, IJob>>> _toolBarChart;

        #endregion END Matrix (Bar Chart) Of TimeSlots and InProcessJobs

        #endregion END Private Fields

        #region Solver Helpers

        private int _currentShiftIndex = -1;
        private IShift _currentShift;

        private int ProcessJobs(int iteration)
        {
            while (_availableJobs.NotEmpty)
            {
                try
                {
                    iteration = GetIterationShiftResources(iteration);

                    var job = _availableJobs.Peek();
                    if (!AreResourcesAvailable(job, iteration))
                    {
                        if (job.FirstPossibleStart == -1)
                        {
                            job.FirstPossibleStart = iteration;
                        }
                        job.IterationAttempts++;
                        
                        // incrememnt counter to next job expiratioin timepoint
                        if (_availableJobs.ProcessingJobs.Count == 0) break;
                        return _availableJobs.ProcessingJobs.Min(x => x.ScheduledEndTime) - 1;
                    }

                    job = _availableJobs.Dequeue(iteration);

                    _shiftManager.ScheduleJob(_currentShift, job);

                    var workers = new HashSet<IScheduledWorker>();
                    var tools = new HashSet<IScheduledTool>();
                    var zone = _zones.FirstOrDefault(x => x == job.Location);
                    if (zone == null)
                    {
                        zone = new Zone("NA", 100);
                    }

                    var exclusionZones = new HashSet<IScheduledZone>();

                    var scheduledZone = new ScheduledZone(zone, iteration, job.ScheduledEndTime, job);
                    var inWorkInstance = new ScheduledJob(job, workers, tools, scheduledZone, exclusionZones, iteration);

                    // create a new entry in our timepoint event tracker
                    List<InWorkEvent> newEvents;
                    if (!_timepoints.TryGetValue(inWorkInstance.End, out newEvents))
                    {
                        newEvents = new List<InWorkEvent>();
                        _timepoints.Add(inWorkInstance.End, newEvents);
                    }

                    var inWorkEvent = new InWorkEvent(_currentShift, inWorkInstance);
                    newEvents.Add(inWorkEvent);
                    _inWorkEvents.Add(inWorkEvent);

                    if (zone != null)
                    {
                        zone.OccupySpot(inWorkEvent);
                    }
                }
                catch (Exception ex)
                {
                    string test = ex.Message;
                }
            }

            return iteration;
        }

        private int GetMinimumShiftCapacityForJob(IJob job)
        {
            // for now, lets assume the minimum amount of non overtime buffer
            // needed to start a job is 75% of the job's duration
            return job.Duration*3/4;
        }

        private const int TotalShiftIncrements = 24*60;

        private int GetIterationShiftTime(int iteration)
        {
            return iteration%TotalShiftIncrements;
        }

        private int GetRemainingIterationShiftTime(IShift shift, int iteration)
        {
            return shift.EndTime - GetIterationShiftTime(iteration);
        }

        private int GetIterationShiftResources(int iteration)
        {
            int iterationShiftTime = GetIterationShiftTime(iteration);

            // do we need to change shifts?
            if (_currentShift.EndTime < iterationShiftTime)
            {
                _currentShiftIndex++;
                if (_currentShiftIndex == _shifts.Length)
                {
                    _currentShiftIndex = 0;
                }

                // finish all jobs currently in progress
                CompleteEndOfShiftJobs();

                // advance interation to beginning of current shift
                var nextIteration = InitializeNextIteration(_currentShift);
                return nextIteration;
            }

            return iteration;
        }

        private bool AreResourcesAvailable(IJob job, int iteration, bool canSpreadJobToNextShift = false)
        {
            return
                   DoesCurrentShiftHaveCapacity(job, iteration, canSpreadJobToNextShift)
                && AreWorkersAvailable(job) 
                && AreToolsAvailable(job) 
                && AreZonesAvailable(job);
        }

        private bool DoesCurrentShiftHaveCapacity(IJob job, int iteration, bool canSpreadJobToNextShift = false)
        {
            // are we constraining on shift schedule?
            if (canSpreadJobToNextShift)
            {
                // we're not constraining on shift schedule so we can start this job in this shift and finish in a later shift
                return true;
            }

            // we have availability
            // but we're constraining on shift schedule so we need to see if we have enough time left in the shift to start this job

            // 1. get how much remaining shift time is required to schedule this job
            int jobStartThreshold = GetMinimumShiftCapacityForJob(job);

            // 2. how much shift time to we have left?
            int currentRemainingShiftTime = GetRemainingIterationShiftTime(_currentShift, iteration);

            // 3. we can only schedule this job if we have enough time left in the curren sprint
            return currentRemainingShiftTime >= jobStartThreshold;
        }

        private bool AreWorkersAvailable(IJob job)
        {
            // if we're not constraining on labor, always return true
            if (job.RequiredSkillSet.Count == 0) return true;

            // are we all busy processing jobs?
            if (!_shiftManager.AreWorkersAvailable(_currentShift, job)) return false;

            return true;
        }

        private bool AreToolsAvailable(IJob job)
        {
            if (job.RequiredToolSet.Count == 0) return true;

            if (!_shiftManager.AreToolsAvailable(_currentShift, job)) return false;

            return true;
        }

        private bool AreZonesAvailable(IJob job)
        {
            if (job.Location == null) return true;
            return _dataManager.ZoneManager.CanWorkJob(job);
        }

        private void ProcessCompletingJobs(int iteration)
        {
            List<InWorkEvent> expiringEvents;
            if (_timepoints.TryGetValue(iteration, out expiringEvents))
            {
                foreach (var inWorkEvent in expiringEvents)
                {
                    ProcessCompletionEvent(inWorkEvent);
                }
            }
        }

        private void ProcessCompletionEvent(InWorkEvent inWorkEvent)
        {
            try
            {
                // put shift resources back into availability pool
                _shiftManager.CompleteJob(inWorkEvent.Shift, inWorkEvent.Owner);

                var completedJob = inWorkEvent.Owner.Job;

                // job is complete, update status
                _availableJobs.Complete(completedJob, inWorkEvent.End);

                // remove job from jobs needing to be processed
                _jobs.Remove(completedJob);
                _inWorkEvents.Remove(inWorkEvent);

                var zone = inWorkEvent.Owner.LocationZone;
                if (zone != null)
                {
                    zone.Resource.WorkCompleted(inWorkEvent);
                }

                // check to see if the completing job has successors that are ready to be worked
                foreach (var job in completedJob.Successors.OrderBy(x => x.Duration))
                {
                    var ready = job.Predecessors.All(x => _availableJobs.ProcessedJobs.ContainsKey(x.ID));
                    if (ready)
                    {
                        _availableJobs.Enqueue(job);
                    }
                }
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }

        private InWorkEvent GetJobWorkEvent(IJob job)
        {
            return _inWorkEvents.FirstOrDefault(x => x.Owner.Job.ID == job.ID);
        }

        private void CompleteEndOfShiftJobs()
        {
            foreach (var job in _availableJobs.ProcessingJobs.ToList())
            {
                var inWorkEvent = GetJobWorkEvent(job);
                if (inWorkEvent != null)
                {
                    ProcessCompletingJobs(inWorkEvent.End);
                }
            }
        }

        private IEnumerable<IScheduledJob> BuildSchedule()
        {
            var schedule = new List<IScheduledJob>();
            var labors = new List<IWorker>();
            foreach (var jobList in _timepoints.Values)
            {
                schedule.AddRange(jobList.Select(job => job.Owner));
            }

            return schedule.OrderBy(x => x.Start);
        }

        private IEnumerable<IScheduledWorker> BuildResourceSchedule()
        {
            var schedule = new List<IScheduledJob>();
            var labors = new List<IScheduledWorker>();

            return labors;
        }

        #endregion END Solver Helpers

        #region Setup Internal Data Structures

        private void SetupJobs(IEnumerable<IJob> jobs)
        {
            //_completedJobs = new Dictionary<int, IJob>();
            _jobs = new List<IJob>(jobs);
            _availableJobs = new JobProcessorList(_jobPriorityFunction);

            foreach (var job in _jobs.Where(x => x.Predecessors.Count == 0).OrderBy(x => x.Duration))
            {
                _availableJobs.Enqueue(job);
            }
        }

        /// <summary>
        /// Collection of worker instances that are associated with a shift
        /// </summary>
        /// <param name="shifts">All shifts with workers assigned to them</param>
        /// <param name="workers">All workers with various skill types assigned to a specific shift</param>
        private void SetupWorkers(List<IShift> shifts, List<IWorker> workers)
        {
            //_shiftWorkers = new Dictionary<IShift, Dictionary<string, Stack<IWorker>>>();
            //_workerBarChart = new Dictionary<IShift, Dictionary<int, Dictionary<IWorker, IJob>>>();

            //foreach (var shift in shifts)
            //{
            //    var shiftWorkersList = new Dictionary<string, Stack<IWorker>>();
            //    _shiftWorkers.Add(shift, shiftWorkersList);
            //    var shiftWorkers = workers.Where(x => x.Shift.Name == shift.Name).ToList();
            //    var shiftWorkerTypes = shiftWorkers.Select(x => x.Skill.SkillType).Distinct();

            //    foreach (var workerType in shiftWorkerTypes)
            //    {
            //        shiftWorkersList.Add(workerType, new Stack<IWorker>());
            //    }

            //    foreach (var worker in shiftWorkers)
            //    {
            //        shiftWorkersList[worker.Skill.SkillType].Push(worker);
            //    }
            //}
        }

        /// <summary>
        /// Collection of tool instances that are associated with a shift
        /// </summary>
        /// <param name="shifts">All shifts with tools assigned to them</param>
        /// <param name="tools">All tools with various tool types assigned to a specific shift</param>
        private void SetupTools(List<IShift> shifts, List<ITool> tools)
        {
            //_shiftTools = new Dictionary<IShift, Dictionary<string, Stack<IToolInstance>>>();
            //_toolBarChart = new Dictionary<IShift, Dictionary<int, Dictionary<ITool, IJob>>>();

            //foreach (var shift in shifts)
            //{
            //    _toolBarChart.Add(shift, new Dictionary<int, Dictionary<ITool, IJob>>());
            //    var shiftToolsList = new Dictionary<string, Stack<IToolInstance>>();
            //    _shiftTools.Add(shift, shiftToolsList);

            //    foreach (var tool in _shiftManager.ShiftToolManager[shift].GetTools())
            //    {
            //        var toolType = tool.ToolType;
            //        shiftToolsList.Add(toolType, new Stack<IToolInstance>());
            //        foreach (var ti in tool.ToolInstances)
            //        {
            //            shiftToolsList[tool.ToolType].Push(ti);
            //        }
            //    }
            //}
        }

        private void SetupZones(IEnumerable<IZone> zones)
        {
            _zones = new List<IZone>(zones);
        }

        #endregion END Setup Internal Data Structures
    }
}