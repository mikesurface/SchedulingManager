using System.Collections.Generic;
using DataSetServices.Data.Modeling.Core;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces
{
    public interface IResourceSchedulingEventCollection<TTaskType, TResourceDefinition>
        where TTaskType : IActivity
        where TResourceDefinition : IResource
    {
        /// <summary>
        /// Resource Type Definition used to perform tasks of type TTaskType
        /// </summary>
        TResourceDefinition Resource { get; }

        void AddResourceInstanceUtilization(int utilizationPercentage);

        // Summary Data
        int AvailableCapacity { get; set; }
        int AvailableCapacityHours { get; set; }

        // Calculated Summary Data
        int UsedCapacity { get; }
        int UsedCapacityHours { get; }
        int Utilization { get; }
        int SolverUtilization { get; }

        IEnumerable<ISchedulingEvent<TTaskType, TResourceDefinition>> GetAllReservations();
        void AddResourceReservation(ISchedulingEvent<TTaskType, TResourceDefinition> reservation);
    }
}