using System.Collections.Generic;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces;
using DataSetServices.Data.Modeling.Core;
using DataSetServices.Data.Modeling.Resources;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling
{
    public class ProjectSchedulingEventManager : IProjectSchedulingEventManager
    {
        #region Constructor

        public ProjectSchedulingEventManager()
        {
            _activitySchedulingManager = new ActivitySchedulingEventManager();
            _laborSchedules = new Dictionary<ILabor, IActivityLaborSchedulingEventCollection>();
            _toolSchedules = new Dictionary<ITool, IActivityToolSchedulingEventCollection>();
        }

        #endregion END Constructor

        #region Activity Schedule Management

        private readonly IActivitySchedulingEventManager _activitySchedulingManager;

        #region Activity Schedules

        public int TotalJobsCount
        {
            get { return ActivitySchedulingManager.TotalJobsCount; }
        }

        public void AddActivitySchedule(IActivitySchedule activitySchedule)
        {
            _activitySchedulingManager.AddActivitySchedule(activitySchedule);
        }

        public IEnumerable<IActivitySchedule> GetActivitySchedules()
        {
            return new List<IActivitySchedule>();
            //return _activitySchedulingManager.GetActivitySchedules();
        }

        public IActivitySchedule GetActivitySchedule(IActivity activity)
        {
            return _activitySchedulingManager.GetActivitySchedule(activity);
        }

        #endregion END Activity Schedules

        #region Activity Scheduling Events

        public int TotalScheduledJobsCount
        {
            get { return ActivitySchedulingManager.TotalScheduledJobsCount; }
        }

        public IActivitySchedulingEventManager ActivitySchedulingManager
        {
            get { return _activitySchedulingManager; }
        }

        public void AddSchedulingEvent(IActivitySchedulingEvent<IResource> reservation)
        {
            ActivitySchedulingManager.AddSchedulingEvent(reservation);
        }

        #endregion END Activity Scheduling Events

        #endregion END Activity Schedule Management

        #region Resource Schedule Management

        #region Resource Schedule Helpers

        private IActivityLaborSchedulingEventCollection GetOrAddLaborSchedule(ILabor labor)
        {
            if (!_laborSchedules.ContainsKey(labor))
            {
                _laborSchedules.Add(labor, new ActivityLaborSchedulingEventCollection(labor));
            }
            return _laborSchedules[labor];
        }

        private IActivityToolSchedulingEventCollection GetOrAddToolSchedule(ITool tool)
        {
            if (!_toolSchedules.ContainsKey(tool))
            {
                _toolSchedules.Add(tool, new ActivityToolSchedulingEventCollection(tool));
            }
            return _toolSchedules[tool];
        }

        #endregion END Resource Schedule Helpers

        #region Resource Capacity

        public void AddResourceCapacity(IResource resource, int availableQuantity, int availableHours)
        {
            var labor = resource as ILabor;
            if (labor != null)
            {
                AddLaborResourceCapacity(labor, availableQuantity, availableHours);
                return;
            }

            var tool = resource as ITool;
            if (tool != null)
            {
                AddToolResourceCapacity(tool, availableQuantity, availableHours);
                return;
            }
        }

        private void AddLaborResourceCapacity(ILabor labor, int availableQuantity, int availableHours)
        {
            var schedule = GetOrAddLaborSchedule(labor);
            schedule.AvailableCapacity = availableQuantity;
            schedule.AvailableCapacityHours = availableHours;
        }

        private void AddToolResourceCapacity(ITool tool, int availableQuantity, int availableHours)
        {
            var schedule = GetOrAddToolSchedule(tool);
            schedule.AvailableCapacity = availableQuantity;
            schedule.AvailableCapacityHours = availableHours;
        }

        #endregion END Resource Capacity

        #region Resource Utilization

        public void AddResourceUtilization(IResource resource, int utilization)
        {
            var labor = resource as ILabor;
            if (labor != null)
            {
                AddLaborResourceUtilization(labor, utilization);
                return;
            }

            var tool = resource as ITool;
            if (tool != null)
            {
                AddToolResourceUtilization(tool, utilization);
                return;
            }
        }

        private void AddLaborResourceUtilization(ILabor labor, int utilization)
        {
            var schedule = GetOrAddLaborSchedule(labor);
            schedule.AddResourceInstanceUtilization(utilization);
        }

        private void AddToolResourceUtilization(ITool tool, int utilization)
        {
            var schedule = GetOrAddToolSchedule(tool);
            schedule.AddResourceInstanceUtilization(utilization);
        }

        #endregion END Resource Utilization

        #region Labor Schedule Management

        private readonly Dictionary<ILabor, IActivityLaborSchedulingEventCollection> _laborSchedules;
        public IDictionary<ILabor, IActivityLaborSchedulingEventCollection> ActivityLaborSchedules
        {
            get { return _laborSchedules; }
        }

        public void AddLaborReservationToSchedule(int instanceId, IActivity activity, ILabor laborType, int startingOffsetInMinutes, int finishingOffsetInMinutes)
        {
            var schedule = new ActivitySchedulingEvent<ILabor>(instanceId, activity, laborType, startingOffsetInMinutes, finishingOffsetInMinutes);

            // add the scheduling event to the activity schedule
            AddSchedulingEvent(schedule);

            // and add the same scheduling event to the labor schedule
            var laborSchedule = GetOrAddLaborSchedule(laborType);
            laborSchedule.AddResourceReservation(schedule);
        }

        #endregion END Labor Schedule Management

        #region Tool Schedule Management

        private readonly Dictionary<ITool, IActivityToolSchedulingEventCollection> _toolSchedules;
        public IDictionary<ITool, IActivityToolSchedulingEventCollection> ActivityToolSchedules
        {
            get { return _toolSchedules; }
        }

        public void AddToolReservationToSchedule(int instanceId, IActivity activity, ITool toolType, int startingOffsetInMinutes, int finishingOffsetInMinutes)
        {
            var schedule = new ActivitySchedulingEvent<ITool>(instanceId, activity, toolType, startingOffsetInMinutes, finishingOffsetInMinutes);

            // add the scheduling event to the activity schedule
            AddSchedulingEvent(schedule);

            // and add the same scheduling event to the tool schedule
            var toolSchedule = GetOrAddToolSchedule(toolType);
            toolSchedule.AddResourceReservation(schedule);
        }

        #endregion END Tool Schedule Management

        #endregion END Resource Schedule Management
    }
}