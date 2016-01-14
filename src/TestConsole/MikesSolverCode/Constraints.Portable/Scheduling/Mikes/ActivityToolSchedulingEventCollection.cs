using DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core;
using DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces;
using DataSetServices.Data.Modeling.Resources;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling
{
    public class ActivityToolSchedulingEventCollection : ActivityResourceSchedulingEventCollection<ITool>, IActivityToolSchedulingEventCollection
    {
        public ActivityToolSchedulingEventCollection(ITool resource) : base(resource)
        {
        }
    }
}