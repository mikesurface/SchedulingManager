using System.Collections.Generic;
using System.Linq;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces;
using DataSetServices.Data.Modeling.Core;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling
{
    public class ActivitySchedulingEventManager : IActivitySchedulingEventManager
    {
        #region Constructor

        public ActivitySchedulingEventManager()
        {
            _activitySchedules = new Dictionary<IActivity, IActivitySchedule>();
            _activitySchedulingEvents = new Dictionary<IActivity, HashSet<IActivitySchedulingEvent<IResource>>>();
        }

        #endregion END Constructor

        #region Activity Schedules

        public int TotalJobsCount
        {
            get { return _activitySchedules.Count; }
        }

        private readonly Dictionary<IActivity, IActivitySchedule> _activitySchedules;

        public void AddActivitySchedule(IActivitySchedule activitySchedule)
        {
            if (!_activitySchedules.ContainsKey(activitySchedule.ScheduledItem))
            {
                _activitySchedules.Add(activitySchedule.ScheduledItem, null);
            }
            _activitySchedules[activitySchedule.ScheduledItem] = activitySchedule;
        }

        public IEnumerable<IActivitySchedule> GetActivitySchedules()
        {
            return new List<IActivitySchedule>();
            //return _activitySchedules.Values.OrderBy(x => x.EarliestFinish.AsMinutes);
        }

        public IActivitySchedule GetActivitySchedule(IActivity activity)
        {
            IActivitySchedule schedule;
            _activitySchedules.TryGetValue(activity, out schedule);
            return schedule;
        }

        #endregion END Activity Schedules

        #region Activity Scheduling Events

        public int TotalScheduledJobsCount
        {
            get { return _activitySchedulingEvents.Count; }
        }

        private readonly Dictionary<IActivity, HashSet<IActivitySchedulingEvent<IResource>>> _activitySchedulingEvents;

        private HashSet<IActivitySchedulingEvent<IResource>> GetOrAddActivity(IActivity activity)
        {
            HashSet<IActivitySchedulingEvent<IResource>> schedules;
            if (!_activitySchedulingEvents.TryGetValue(activity, out schedules))
            {
                schedules = new HashSet<IActivitySchedulingEvent<IResource>>();
                _activitySchedulingEvents.Add(activity, schedules);
            }
            return schedules;
        }

        public void AddSchedulingEvent(IActivitySchedulingEvent<IResource> reservation)
        {
            GetOrAddActivity(reservation.Activity).Add(reservation);
        }

        #endregion END Activity Scheduling Events
    }
}