using System;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Core.BaseTypes;
using DataSetServices.Data.Domain;
using System.Collections.Generic;
using System.Linq;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Scheduling
{
    public class TaskSchedule<T> : ITaskSchedule<T>
        where T : ITask
    {
        public TaskSchedule(T task)
        {
            Task = task;

            Float = new TimeFrame(TimeFrameUnits.Hours, 0);                 // Float == 0 ==> on critical path

            EarliestStart = new TimeFrame(TimeFrameUnits.Hours, 0);
            EarliestFinish = new TimeFrame(TimeFrameUnits.Hours, 0);
            LatestStart = new TimeFrame(TimeFrameUnits.Hours, 0);
            LatestFinish = new TimeFrame(TimeFrameUnits.Hours, 0);

            ScheduledStart = new TimeFrame(TimeFrameUnits.Hours, 0);
            ScheduledFinish = new TimeFrame(TimeFrameUnits.Hours, 0);
            
            AbsoluteActiveTimes = new List<TimePeriod>();

            if (Task != null && Task.Duration != null)
            {
                TargetDuration = new TimeFrame(Task.Duration.BaseLine.AsSeconds.ToString());
            }
            else
            {
                TargetDuration = new TimeFrame(TimeFrameUnits.Hours, 0);
            }
        }

        public TaskSchedule()
            : this(default(T))
        { }

        public T Task { get; set; }

        public bool OnCriticalPath
        {
            get { return (Math.Abs(Float.AsSeconds) < Double.Epsilon); } 
        }

        public TimeFrame EarliestStart { get; set; }
        public TimeFrame EarliestFinish { get; set; }
        public TimeFrame LatestStart { get; set; }
        public TimeFrame LatestFinish { get; set; }

        public TimeFrame Float { get;  set; }

        public TimeFrame ScheduledStart { get; set; }
        public TimeFrame ScheduledFinish { get; set; }
        public TimeFrame TargetDuration { get; set; }

        public List<TimePeriod> AbsoluteActiveTimes { get; set; }

        public IEnumerable<TimePeriod> RelativeActiveTimes
        {
            get
            {
                return AbsoluteActiveTimes.Select(time => new TimePeriod(time.Start - ScheduledStart, time.Duration));
            }
        }

        public IEnumerable<TimePeriod> AbsoluteIdleTimes
        {
            get
            {
                List<TimePeriod> idleTime = new List<TimePeriod>();

                int start = 0, end = 0;
                bool skippedFirst = false;
                foreach (var active in AbsoluteActiveTimes)
                {
                    var lastend = end;

                    start = (int)active.Start.AsMinutes;
                    end = (int)active.Finish.AsMinutes;

                    if (skippedFirst)
                    {
                        var idle = new TimePeriod(TimeFrame.FromMinutes(lastend), TimeFrame.FromMinutes(start - lastend));
                        idleTime.Add(idle);
                    }
                    else
                    {
                        skippedFirst = true;
                    }
                }
                return idleTime;
            }
        }

        public IEnumerable<TimePeriod> RelativeIdleTimes
        {
            get
            {
                return AbsoluteIdleTimes.Select(time => new TimePeriod(time.Start - ScheduledStart, time.Duration));
            }
        }

        public static cType FromScheduleToType<cType>(ITaskSchedule source) where cType : TaskSchedule<T>, new()
        {
            cType copy = new cType();
      
            copy.EarliestStart = source.EarliestStart;
            copy.EarliestFinish = source.EarliestFinish;
            copy.LatestStart = source.LatestStart;
            copy.LatestFinish = source.LatestFinish;

            copy.ScheduledStart = source.ScheduledStart;
            copy.ScheduledFinish = source.ScheduledFinish;

            copy.TargetDuration = TimeFrame.FromSeconds(source.TargetDuration.AsSeconds);
            foreach(var timePeriod in source.AbsoluteActiveTimes)
            {
                copy.AbsoluteActiveTimes.Add(timePeriod);
            }

            return copy;
        }
    }
}
