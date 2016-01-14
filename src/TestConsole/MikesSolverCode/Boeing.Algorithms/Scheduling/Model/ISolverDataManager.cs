using System.Collections.Generic;
using System.Threading.Tasks;
using Boeing.Algorithms.Scheduling.Core;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Analytics.ConstraintSolver.Solvers;

namespace Boeing.Algorithms.Scheduling.Model
{
    public interface ISolverDataManager
    {
        Task<ConstraintSolverResult> InitializeDataAsync();
        Task<SolverResultState> ProcessSolution(IEnumerable<IScheduledJob> scheduledJobs);
    }
}