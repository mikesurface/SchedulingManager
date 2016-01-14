using System;
using System.Collections.Generic;
using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Resources.Tools;
using Boeing.Algorithms.Scheduling.Model.Resources.Workers;
using Boeing.Algorithms.Scheduling.Model.Resources.Zones;

namespace Boeing.Algorithms.Scheduling.Model.Jobs
{
    public interface IJob : IEntity<int>
    {
        Guid SchedulableItemUID { get; }
        int Duration { get; }
        int Slack { get; }
        IZone Location { get; }
        HashSet<ISkill> RequiredSkillSet { get; }
        HashSet<ITool> RequiredToolSet { get; }
        HashSet<IJob> Predecessors { get; }
        HashSet<IJob> Successors { get; }

        int FirstPossibleStart { get; set; }
        int IterationAttempts { get; set; }

        void ScheduleJob(int start);
        int ScheduledStartTime { get; }
        int ScheduledEndTime { get; }
    }
}