using DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces;
using DataSetServices.Data.Modeling.Core;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core
{
    public abstract class ActivityResourceSchedulingEventCollection<TResourceDefinition>
        : SchedulingEventCollection<IActivity, TResourceDefinition>, IActivityResourceSchedulingEventCollection<TResourceDefinition> 
        where TResourceDefinition : IResource
    {
        protected ActivityResourceSchedulingEventCollection(TResourceDefinition resource) : base(resource)
        {
        }
    }
}