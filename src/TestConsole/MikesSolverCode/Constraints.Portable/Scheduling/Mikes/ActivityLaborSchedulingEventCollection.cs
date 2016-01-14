using DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core;
using DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces;
using DataSetServices.Data.Modeling.Resources;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling
{
    public class ActivityLaborSchedulingEventCollection : ActivityResourceSchedulingEventCollection<ILabor>, IActivityLaborSchedulingEventCollection
    {
        public ActivityLaborSchedulingEventCollection(ILabor resource) : base(resource)
        {
        }
    }
}