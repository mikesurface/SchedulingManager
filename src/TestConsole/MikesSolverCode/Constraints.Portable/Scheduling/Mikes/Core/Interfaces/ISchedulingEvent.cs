using DataSetServices.Data.Core.BaseTypes;
using DataSetServices.Data.Modeling.Core;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces
{
    public interface ISchedulingEvent
    {
        TimeFrame ScheduledStart { get; }
        TimeFrame ScheduledFinish { get; }
    }

    public interface ISchedulingEvent<out TTaskType, out TResourceType> : ISchedulingEvent
        where TTaskType : IActivity
        where TResourceType : IResource
    {
        /// <summary>
        /// Activity that was scheduled
        /// </summary>
        TTaskType Activity { get; }

        /// <summary>
        /// Resource that was used to perform the activity
        /// </summary>
        TResourceType Resource { get; }

        int ResourceInstanceId { get; }
    }
}