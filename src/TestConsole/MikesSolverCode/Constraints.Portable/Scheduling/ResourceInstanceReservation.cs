using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Core.BaseTypes;
using DataSetServices.Data.Modeling.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling
{
    public class ResourceInstanceReservation : IResourceInstanceReservation
    {
        public int ResourceInstanceID { get; private set; }

        public IResource Resource { get; private set; }

        public IActivitySchedule ActivitySchedule { get; private set; }

        public TimeFrame ScheduledStart { get; private set; }

        public TimeFrame ScheduledFinish { get; private set; }

        public ResourceInstanceReservation (int resourceInstanceID, 
            IResource resource, 
            IActivitySchedule activitySchedule,
            TimeFrame scheduledStart, 
            TimeFrame scheduledFinish)
        {
            ResourceInstanceID = resourceInstanceID;
            Resource = resource;
            ActivitySchedule = activitySchedule;
            ScheduledStart = scheduledStart;
            ScheduledFinish = scheduledFinish;
        }
    }
}
