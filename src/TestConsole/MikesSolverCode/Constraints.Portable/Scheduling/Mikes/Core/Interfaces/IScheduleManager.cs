using System.Collections.Generic;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Modeling.Core;
using DataSetServices.Data.Modeling.Resources;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces
{
    public interface IScheduleManager
    {
        IProjectSchedulingEventManager ProjectSchedulingManager { get; }
        
        void AddResourceUtilization(IResource resource, int utilization);
        void AddLaborReservationToSchedule(int instanceId, IActivity activity, ILabor laborType, int start, int finish);
        void AddToolReservationToSchedule(int instanceId, IActivity activity, ITool toolType, int start, int finish);

        // putting it here as a copy until this can be refactored into the IProjectSchedulingEventManager's ActivitySchedules collection
        List<IActivitySchedule> ActivitySchedules { get; }
        void AddActivitySchedule(IActivitySchedule activitySchedule);
    }
}