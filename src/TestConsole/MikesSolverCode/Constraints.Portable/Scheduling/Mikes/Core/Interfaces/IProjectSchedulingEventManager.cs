using System.Collections.Generic;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Modeling.Core;
using DataSetServices.Data.Modeling.Resources;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces
{
    public interface IProjectSchedulingEventManager
    {
        IActivitySchedulingEventManager ActivitySchedulingManager { get; }
        
        int TotalJobsCount { get; }
        void AddActivitySchedule(IActivitySchedule activitySchedule);
        IEnumerable<IActivitySchedule> GetActivitySchedules();
        IActivitySchedule GetActivitySchedule(IActivity activity);

        int TotalScheduledJobsCount { get; }
        void AddSchedulingEvent(IActivitySchedulingEvent<IResource> reservation);

        void AddResourceCapacity(IResource resource, int availableQuantity, int availableHours);
        void AddResourceUtilization(IResource resource, int utilization);

        IDictionary<ILabor, IActivityLaborSchedulingEventCollection> ActivityLaborSchedules { get; }
        void AddLaborReservationToSchedule(int instanceId, IActivity activity, ILabor laborType, int startingOffsetInMinutes, int finishingOffsetInMinutes);

        IDictionary<ITool, IActivityToolSchedulingEventCollection> ActivityToolSchedules { get; }
        void AddToolReservationToSchedule(int instanceId, IActivity activity, ITool toolType, int startingOffsetInMinutes, int finishingOffsetInMinutes);
    }
}