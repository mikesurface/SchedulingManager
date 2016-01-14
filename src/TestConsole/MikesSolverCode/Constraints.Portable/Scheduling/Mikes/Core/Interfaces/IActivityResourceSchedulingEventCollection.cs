using DataSetServices.Data.Modeling.Core;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces
{
    public interface IActivityResourceSchedulingEventCollection<TResourceDefinition> : IResourceSchedulingEventCollection<IActivity, TResourceDefinition>
        where TResourceDefinition : IResource
    {
    }
}