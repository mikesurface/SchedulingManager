using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Modeling.Core;
using DataSetServices.Data.Modeling.Projects;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces
{
    public interface IScheduleAnalysisRequest : IAnalysisRequest<ProjectDataSet>
    {
        IProjectSchedulingEventManager SchedulingManager { get; }
        int GetResourceTotalAvailability(IResource resource);
        int GetResourceTotalHoursAvailability(IResource resource);
        bool ContainsJobKey(int jobId);
        int TotalJobsCount { get; }
        int TotalSkillsCount { get; }
        int TotalToolsCount { get; }
        int TotalSpacesCount { get; }
    }
}