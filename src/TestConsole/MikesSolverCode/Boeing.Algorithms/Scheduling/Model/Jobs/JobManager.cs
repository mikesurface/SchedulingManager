using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Modeling.Core;

namespace Boeing.Algorithms.Scheduling.Model.Jobs
{
    public class JobManager : IJobManager
    {
        #region Constructor and Private Fields

        public JobManager(JobConverter jobConverter)
        {
            _jobConverter = jobConverter;
        }

        private readonly JobConverter _jobConverter;
        private ConcurrentDictionary<Guid, IActivitySchedule> _schedulableActivities;

        #endregion END Constructor and Private Fields

        #region IJobManager Logic

        public List<IJob> RootJobs { get; private set; }

        public void ProcessModelActivities(List<IActivitySchedule> activities)
        {
            RootJobs = new List<IJob>();
            _schedulableActivities = new ConcurrentDictionary<Guid, IActivitySchedule>(activities.ToDictionary(x => x.ScheduledItem.UID, x => x));
            foreach (var activity in activities.Where(x => x.ScheduledItem.DefaultPredecessors.Count == 0))
            {
                ProcessSchedulableActivity(activity);
            }
        }

        public IEnumerable<IJob> GetJobs()
        {
            return _jobConverter.InstancesMap.Values;
        }

        #endregion END IJobManager Logic

        #region Recursive Processing Logic

        private void ProcessSchedulableActivity(IActivitySchedule activity)
        {
            var rootJob = ProcessActivity(activity.ScheduledItem);
            RootJobs.Add(rootJob);
        }

        private IJob ProcessActivity(IActivity activity)
        {
            var schedulableActivity = _schedulableActivities[activity.UID];
            if (_jobConverter.InstancesMap.ContainsKey(schedulableActivity))
            {
                return _jobConverter.InstancesMap[schedulableActivity];
            }

            // process into job and walk successors
            var predecessorJob = CreateJobFromActivity(schedulableActivity);
            foreach (var successor in activity.Successors<IActivity>())
            {
                var successorJob = ProcessActivity(successor);
                predecessorJob.Successors.Add(successorJob);
                successorJob.Predecessors.Add(predecessorJob);
            }

            return predecessorJob;
        }

        private IJob CreateJobFromActivity(IActivitySchedule activitySchedule)
        {
            return _jobConverter.Convert(activitySchedule);
        } 

        #endregion END Recursive Processing Logic
    }
}