using DataSetServices.Data.Modeling.Core;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces
{
    public interface IActivitySchedulingEvent<out TResource> : ISchedulingEvent<IActivity, TResource>
        where TResource : IResource
    {
    }
}