using System;
using System.Collections.Generic;
using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Resources.Tools;
using Boeing.Algorithms.Scheduling.Model.Resources.Workers;
using Boeing.Algorithms.Scheduling.Model.Resources.Zones;

namespace Boeing.Algorithms.Scheduling.Model.Jobs
{
    public struct Job : IJob
    {
        #region Constructors

        public Job(int duration) : this(Guid.Empty, duration, 0, null)
        {
        }

        public Job(Guid uid, int duration, int slack, IZone location)
            : this(uid, -1, duration, slack, location)
        {
        }

        private Job(Guid uid, int id, int duration, int slack, IZone location) : this()
        {
            SchedulableItemUID = uid;
            if (id == -1)
            {
                id = this.NextId();
            }
            ID = id;
            Duration = duration;
            Slack = slack;
            Location = location;
            FirstPossibleStart = -1;
            IterationAttempts = 0;

            RequiredSkillSet = new HashSet<ISkill>();
            RequiredToolSet = new HashSet<ITool>();

            Predecessors = new HashSet<IJob>();
            Successors = new HashSet<IJob>();
        }

        #endregion END Constructors

        public Guid SchedulableItemUID { get; private set; }
        public int ID { get; private set; }
        public int Duration { get; private set; }
        public int Slack { get; private set; }
        public IZone Location { get; private set; }

        public HashSet<ISkill> RequiredSkillSet { get; private set; }
        public HashSet<ITool> RequiredToolSet { get; private set; }

        public HashSet<IJob> Predecessors { get; private set; }
        public HashSet<IJob> Successors { get; private set; }

        public int FirstPossibleStart { get; set; }
        public int IterationAttempts { get; set; }

        public void ScheduleJob(int start)
        {
            ScheduledStartTime = start;
            ScheduledEndTime = start + Duration;
        }

        public int ScheduledStartTime { get; private set; }
        public int ScheduledEndTime { get; private set; }

        public override string ToString()
        {
            return string.Format("Job: {0}, Duration: {1}", ID, Duration);
        }
    }
}