using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Core.BaseTypes;
using DataSetServices.Data.Modeling.Activities;
using DataSetServices.Data.Modeling.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling
{
    /// <summary>
    /// Concrete class to be used internally and by solvers only. 
    /// Interface has getters to be exposed to outside world. 
    /// </summary>
    public class ResourceInstanceReservations : IResourceInstanceReservations
    {
        public int ResourceInstanceID { get; private set; }

        public IResource Resource { get; private set; }

        public IEnumerable<IResourceInstanceReservation> Reservations { get; private set; }

        public ResourceInstanceReservations(int resourceInstanceID, IResource resource)
        {
            ResourceInstanceID = resourceInstanceID;
            Resource = resource;
            Reservations = new List<IResourceInstanceReservation>();
        }

        public void AddResourceInstanceReservation(IResourceInstanceReservation resourceInstanceReservation)
        {
            if (resourceInstanceReservation.ResourceInstanceID != ResourceInstanceID)
                throw new Exception(string.Format("Cannot add resource reservation because the resourceInstanceID for this schedule {0} does not match that for what you are trying to add {1}", ResourceInstanceID, resourceInstanceReservation.ResourceInstanceID));

            if (resourceInstanceReservation.Resource.UID != Resource.UID)
                throw new Exception(string.Format("Cannot add resource reservation because the resourceType for this schedule {0} does not match that for what you are trying to add {1}", Resource.Name, resourceInstanceReservation.Resource.Name));

            (Reservations as List<IResourceInstanceReservation>).Add(resourceInstanceReservation);
        }

        public void AddResourceInstanceReservation(TimeFrame scheduledStart, TimeFrame scheduledFinish, IActivitySchedule activitySchedule)
        {
            ResourceInstanceReservation newReservation = new ResourceInstanceReservation(ResourceInstanceID, Resource, activitySchedule, scheduledStart, scheduledFinish);
            (Reservations as List<IResourceInstanceReservation>).Add(newReservation);
        }

        public IEnumerable<IResourceInstanceReservation> GetResourceInstanceReservations(int resourceInstanceID)
        {
            return Reservations.Where(reservation => reservation.ResourceInstanceID == resourceInstanceID);
        }
    }
}
