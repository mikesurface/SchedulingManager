using System.Collections.Generic;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Modeling.Core;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces
{
    public interface IActivitySchedulingEventManager
    {
        int TotalJobsCount { get; }
        int TotalScheduledJobsCount { get; }
        //IEnumerable<IActivitySchedule> GetActivitySchedules();
        IActivitySchedule GetActivitySchedule(IActivity activity);
        void AddActivitySchedule(IActivitySchedule activitySchedule);
        void AddSchedulingEvent(IActivitySchedulingEvent<IResource> reservation);
    }
}