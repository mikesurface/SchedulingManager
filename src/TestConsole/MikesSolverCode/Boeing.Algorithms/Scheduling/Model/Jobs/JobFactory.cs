using System;
using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Resources.Zones;

namespace Boeing.Algorithms.Scheduling.Model.Jobs
{
    public class JobFactory : EntityGenerator<IJob>
    {
        public override IJob Create(params object[] args)
        {
            var uid = Guid.Empty;
            var duration = 10;
            var slack = 0;
            IZone zone = null;
            if (args.Length == 4)
            {
                uid = (Guid) args[0];
                duration = (int) args[1];
                slack = (int) args[2];
                zone = (IZone) args[3];
            }
            return new Job(uid, duration, slack, zone);
        }
    }
}