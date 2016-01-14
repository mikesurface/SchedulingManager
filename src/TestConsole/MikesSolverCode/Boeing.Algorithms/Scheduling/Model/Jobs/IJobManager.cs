using System.Collections.Generic;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;

namespace Boeing.Algorithms.Scheduling.Model.Jobs
{
    public interface IJobManager
    {
        List<IJob> RootJobs { get; }
        void ProcessModelActivities(List<IActivitySchedule> activities);
    }
}