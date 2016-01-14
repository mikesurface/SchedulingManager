using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Modeling.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling
{
    public class SolutionSchedule : ISolutionSchedule
    {
        public SolutionSchedule()
        {
            _shipsetSchedules = new Dictionary<int, IShipsetSchedule>();
            _activitySchedule = new List<IActivitySchedule>();
            _resourceInstanceSchedule = new List<IResourceInstanceReservations>();
        }

        private Dictionary<int, IShipsetSchedule> _shipsetSchedules;
        public Dictionary<int, IShipsetSchedule> ShipsetSchedules
        {
            get
            {
                return _shipsetSchedules;
            }
        }

        private List<IActivitySchedule> _activitySchedule;
        public IEnumerable<IActivitySchedule> ActivitySchedule
        {
            get
            {
                return _activitySchedule;
            }
        }

        private List<IResourceInstanceReservations> _resourceInstanceSchedule;
        public IEnumerable<IResourceInstanceReservations> ResourceInstanceSchedule
        {
            get
            {
                return _resourceInstanceSchedule;
            }
        }

        public IEnumerable<IActivitySchedule> GetCriticalPathActivitySchedule()
        {
            return ActivitySchedule.Where(sched => sched.OnCriticalPath);
        }


        public void AddSolutionActivitySchedule(IActivitySchedule activitySchedule)
        {
            _activitySchedule.Add(activitySchedule);
        }

        public void AddSolutionResourceInstanceReservations(IResourceInstanceReservations instanceReservations)
        {
            _resourceInstanceSchedule.Add(instanceReservations);
        }
    }
}
