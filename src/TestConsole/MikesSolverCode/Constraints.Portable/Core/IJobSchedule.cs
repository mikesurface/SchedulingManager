using DataSetServices.Data.Modeling.Core;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Core
{
    public interface IJobSchedule : IItemSchedule<IActivity>
    {
        IActivity Activity { get; }
        int ShipsetID { get; }
    }
}