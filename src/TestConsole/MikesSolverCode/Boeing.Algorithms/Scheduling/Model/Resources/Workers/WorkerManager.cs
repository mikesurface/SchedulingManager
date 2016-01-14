using System;
using System.Collections.Generic;
using Boeing.Algorithms.Scheduling.Core;
using Boeing.Algorithms.Scheduling.Model.Jobs;
using Boeing.Algorithms.Scheduling.Model.Shifts;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Workers
{
    public class WorkerManager
    {
        public WorkerManager(IShift shift, LaborConverter converter)
        {
            _shift = shift;
            _converter = converter;
            _inProcessJobs = new Dictionary<IWorker, IJob>();
            _inProcessWorkers = new Dictionary<IJob, HashSet<IWorker>>();
            _inProcessWorkerSchedules = new Dictionary<IJob, HashSet<IScheduledWorker>>();
            _processedJobs = new Dictionary<IJob, Tuple<int, int>>();
            _workerAvailableStates = new Dictionary<ISkill, Dictionary<int, bool>>();
            foreach (var item in _converter.WorkerTypes)
            {
                _workerAvailableStates.Add(item.Key, new Dictionary<int, bool>());
                foreach (var worker in item.Value)
                {
                    _workerAvailableStates[item.Key].Add(worker.Key, true);
                }
            }
        }

        private readonly IShift _shift;
        private readonly LaborConverter _converter;

        private readonly Dictionary<IWorker, IJob> _inProcessJobs;
        private readonly Dictionary<IJob, HashSet<IWorker>> _inProcessWorkers;
        private readonly Dictionary<IJob, HashSet<IScheduledWorker>> _inProcessWorkerSchedules;
        private readonly Dictionary<IJob, Tuple<int, int>> _processedJobs;

        private readonly Dictionary<ISkill, Dictionary<int, bool>> _workerAvailableStates;

        public IEnumerable<IWorker> GetWorkers()
        {
            return _converter.Instances.Values;
        }

        public IEnumerable<IJob> GetProcessedJobs()
        {
            return _processedJobs.Keys;
        }

        public IEnumerable<IJob> GetInProcessJobs()
        {
            return _inProcessJobs.Values;
        }

        public IEnumerable<IWorker> GetBusyWorkers()
        {
            return _inProcessJobs.Keys;
        }

        private IWorker GetNextWorker(ISkill skill)
        {
            foreach (var workerState in _workerAvailableStates[skill])
            {
                if (workerState.Value)
                {
                    // return first worker that is available
                    return _converter.WorkerTypes[skill][workerState.Key];
                }
            }
            return null;
        }

        public HashSet<IScheduledWorker> ScheduleJob(IJob job)
        {
            if (_inProcessWorkers.ContainsKey(job)) return null;

            var workers = new HashSet<IWorker>();
            var scheduledWorkers = new HashSet<IScheduledWorker>();
            _inProcessWorkers.Add(job, workers);
            _inProcessWorkerSchedules.Add(job, scheduledWorkers);

            foreach (var skill in job.RequiredSkillSet)
            {
                var worker = GetNextWorker(skill);
                if (worker == null) return null;
                var scheduledWorker = new ScheduledWorker(worker, job.ScheduledStartTime, job.ScheduledEndTime, job);
                scheduledWorkers.Add(scheduledWorker);
                workers.Add(worker);
                _inProcessJobs.Add(worker, job);
                _workerAvailableStates[skill][worker.ID] = false;
            }
            return scheduledWorkers;
        }

        public void CompleteJob(IJob job)
        {
            if (_inProcessWorkers.ContainsKey(job))
            {
                foreach (var worker in _inProcessWorkers[job])
                {
                    _inProcessJobs.Remove(worker);
                    _workerAvailableStates[worker.Skill][worker.ID] = true;
                }
                if (!_processedJobs.ContainsKey(job))
                {
                    _processedJobs.Add(job, new Tuple<int, int>(job.ScheduledStartTime, job.ScheduledEndTime));
                }
                _inProcessWorkers.Remove(job);
            }
        }

        public bool CanWork(IJob job)
        {
            foreach (var skill in job.RequiredSkillSet)
            {
                var next = GetNextWorker(skill);
                if (next == null) return false;
            }
            return true;
        }
    }
}