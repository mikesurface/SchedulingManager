using System.Collections.Generic;
using Boeing.Algorithms.Scheduling.Model.Jobs;

namespace Boeing.Algorithms.Scheduling.Core
{
    public interface IScheduledJob : IEventable
    {
        IJob Job { get; }
        HashSet<IScheduledWorker> Workers { get; }
        HashSet<IScheduledTool> Tools { get; }
        IScheduledZone LocationZone { get; }
        HashSet<IScheduledZone> ExclusionZones { get; }
    }

}