using System.Collections.Generic;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Core.BaseTypes;
using DataSetServices.Data.Modeling.Activities;
using DataSetServices.Data.Modeling.Core;
using System.Linq;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling
{
    public class ActivitySchedule : TaskSchedule<IActivity>, IActivitySchedule, ITaskSchedule<IActivity>
    {
        public ActivitySchedule()
            :this(null)
        {}

        public ActivitySchedule(IActivity activity)
            : base(activity)
        {
            Activity = activity;
        }

        public IActivity Activity 
        {
            get
            {
                return Task;
        }
            set
            {
                Task = value;
            }
        }

        public int ShipsetID { get; set; }
    }
}
