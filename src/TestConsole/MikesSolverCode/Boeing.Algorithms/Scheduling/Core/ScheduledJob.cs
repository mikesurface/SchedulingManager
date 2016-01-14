using System.Collections.Generic;
using System.Linq;
using Boeing.Algorithms.Scheduling.Model.Jobs;

namespace Boeing.Algorithms.Scheduling.Core
{
    public struct ScheduledJob : IScheduledJob
    {
        #region Constructors

        public ScheduledJob(
            IJob job, 
            HashSet<IScheduledWorker> worker, 
            IScheduledZone locationZone, 
            HashSet<IScheduledZone> exclusionZones, 
            int start)
            : this(job, worker, new HashSet<IScheduledTool>(), locationZone, exclusionZones, start)
        {
        }

        public ScheduledJob(
            IJob job, 
            HashSet<IScheduledWorker> worker, 
            HashSet<IScheduledTool> tool, 
            IScheduledZone locationZone, 
            HashSet<IScheduledZone> exclusionZones, 
            int start)
            : this()
        {
            Job = job;
            Workers = worker;
            Tools = tool;
            LocationZone = locationZone;
            ExclusionZones = exclusionZones;
            FirstAttemptedStart = job.FirstPossibleStart;
            Start = start;
            End = Start + job.Duration;
        }

        #endregion END Constructors

        public IJob Job { get; private set; }
        public HashSet<IScheduledWorker> Workers { get; private set; }
        public HashSet<IScheduledTool> Tools { get; private set; }
        public IScheduledZone LocationZone { get; private set; }
        public HashSet<IScheduledZone> ExclusionZones { get; private set; }

        public int FirstAttemptedStart { get; set; }
        public int Start { get; private set; }
        public int End { get; private set; }

        public override string ToString()
        {
            return string.Format(
                "Job: {0} ({1}), Worker: {2}, Tool: {3}, locationZone: {4}, Start: {5}, End: {6}", 
                Job.ID, 
                Job.Duration, 
                Workers == null ? -1 : Workers.First().Resource.ID,
                Tools == null ? -1 : Tools.First().Resource.ID,
                LocationZone == null ? -1 : LocationZone.Resource.ID, 
                Start, 
                End);
        }
    }
}