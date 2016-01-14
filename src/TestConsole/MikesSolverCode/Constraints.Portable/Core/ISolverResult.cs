using System.Collections.Generic;
using DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Core
{
    public interface ISolverResult
    {
        SolverResultState State { get; }

        IConstraints Constraints { get; }

        List<IConstraintSolverError> Errors { get; }

        IConstraintSolverResultSummary Summary {get; }

        ISolutionSchedule SolutionSchedule { get; }

        List<IActivitySchedule> Schedule { get; }

        ActivityProjectSchedule ActivitySchedule { get; }
        ResourceProjectSchedule ResourceSchedule { get; }

        IActivitySchedulingEventManager ScheduleManager { get; }
    }
}