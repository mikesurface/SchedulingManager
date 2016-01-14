using System.Collections.Generic;
using System.Linq;
using DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core.Interfaces;
using DataSetServices.Data.Modeling.Core;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling.Core
{
    public abstract class SchedulingEventCollection<TTaskType, TResourceDefinition> 
        : IResourceSchedulingEventCollection<TTaskType, TResourceDefinition>
        where TTaskType : IActivity
        where TResourceDefinition : IResource
    {
        #region Constructor

        protected SchedulingEventCollection(TResourceDefinition resource)
        {
            Resource = resource;
            _resourceDefinitionInstances = new Dictionary<int, List<ISchedulingEvent<TTaskType, TResourceDefinition>>>();
            _resourceInstanceUtilizations = new List<int>();
            _reservations = new List<ISchedulingEvent<TTaskType, TResourceDefinition>>();
        }

        #endregion END Constructor

        #region IActivitySchedulingEvents Properties

        public TResourceDefinition Resource { get; protected set; }

        public int AvailableCapacity { get; set; }

        public int AvailableCapacityHours { get; set; }

        public int UsedCapacity
        {
            get { return CalculateCapacityUsed(); }
        }

        public int UsedCapacityHours
        {
            get { return CalculateHoursUsed(); }
        }

        // These are virtual so that a tool scheduling manager can change how it chooses to calculate capacity
        protected virtual int CalculateHoursUsed()
        {
            if (_reservations.Count == 0) return 0;
            return _reservations.Sum(x => (int)x.ScheduledFinish.AsHours - (int)x.ScheduledStart.AsHours);
        }

        // These are virtual so that a tool scheduling manager can change how it chooses to calculate capacity
        protected virtual int CalculateCapacityUsed()
        {
            return _resourceDefinitionInstances.Count;
        }

        public int Utilization
        {
            get { return AvailableCapacityHours == 0 ? 0 : UsedCapacityHours / AvailableCapacityHours * 100; }
        }

        public int SolverUtilization
        {
            get { return _resourceInstanceUtilizations.Count == 0 ? 0 : _resourceInstanceUtilizations.Sum(x => x)/_resourceInstanceUtilizations.Count; }
        }

        private readonly List<int> _resourceInstanceUtilizations;

        public void AddResourceInstanceUtilization(int utilizationPercentage)
        {
            _resourceInstanceUtilizations.Add(utilizationPercentage);
        }

        #endregion END IActivitySchedulingEvents Properties

        #region Resource Scheduling Logic

        private readonly IDictionary<int, List<ISchedulingEvent<TTaskType, TResourceDefinition>>> _resourceDefinitionInstances;

        private readonly List<ISchedulingEvent<TTaskType, TResourceDefinition>> _reservations;

        public void AddResourceReservation(ISchedulingEvent<TTaskType, TResourceDefinition> reservation)
        {
            List<ISchedulingEvent<TTaskType, TResourceDefinition>> workedJobs;
            if (!_resourceDefinitionInstances.TryGetValue(reservation.ResourceInstanceId, out workedJobs))
            {
                workedJobs = new List<ISchedulingEvent<TTaskType, TResourceDefinition>>();
                _resourceDefinitionInstances.Add(reservation.ResourceInstanceId, workedJobs);
            }
            _resourceDefinitionInstances[reservation.ResourceInstanceId].Add(reservation);
            _reservations.Add(reservation);
        }

        public IEnumerable<ISchedulingEvent<TTaskType, TResourceDefinition>> GetAllReservations()
        {
            return _reservations;
        }

        #endregion END Resource Scheduling Logic
    }
}