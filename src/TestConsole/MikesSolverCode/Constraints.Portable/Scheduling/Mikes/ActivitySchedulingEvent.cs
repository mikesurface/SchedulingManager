using DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces;
using DataSetServices.Data.Core.BaseTypes;
using DataSetServices.Data.Modeling.Core;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling
{
    public struct ActivitySchedulingEvent<TResource> : IActivitySchedulingEvent<TResource>
        where TResource : IResource
    {
        #region Constructors

        /// <summary>
        /// Stores a reservation for a specific resource instance for a specific
        /// duration of time as specified between the start and finish offsets
        /// to be allocated to a specific activity
        /// </summary>
        /// <param name="resourceInstanceId">Specific IResource instance to use</param>
        /// <param name="activity">Job or activity being worked on</param>
        /// <param name="resource">Type of resource performing the work</param>
        /// <param name="startOffsetInMinutes">The start time from beginning of schedule</param>
        /// <param name="finishOffsetInMinutes">The finish time from beginning of schedule</param>
        public ActivitySchedulingEvent(
            int resourceInstanceId, 
            IActivity activity, 
            TResource resource, 
            int startOffsetInMinutes, 
            int finishOffsetInMinutes)
            : this(
            resourceInstanceId, 
            activity, 
            resource, 
            TimeFrame.FromMinutes(startOffsetInMinutes), 
            TimeFrame.FromMinutes(finishOffsetInMinutes))
        {
        }

        private ActivitySchedulingEvent(
            int resourceInstanceId, 
            IActivity activity, 
            TResource resource, 
            TimeFrame startOffset, 
            TimeFrame finishOffset) : this()
        {
            ResourceInstanceId = resourceInstanceId;
            Activity = activity;
            Resource = resource;
            ScheduledStart = startOffset;
            ScheduledFinish = finishOffset;
        }

        #endregion END Constructors

        #region ISchedulingEvent Implementation

        public int ResourceInstanceId { get; private set; }
        public IActivity Activity { get; private set; }
        public TResource Resource { get; private set; }
        public TimeFrame ScheduledStart { get; private set; }
        public TimeFrame ScheduledFinish { get; private set; }

        #endregion END ISchedulingEvent Implementation
    }
}