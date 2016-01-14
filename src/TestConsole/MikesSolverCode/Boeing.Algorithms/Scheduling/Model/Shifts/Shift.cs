using System.Collections.Generic;
using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Core;
using Boeing.Algorithms.Scheduling.Model.Resources.Tools;
using Boeing.Algorithms.Scheduling.Model.Resources.Workers;

namespace Boeing.Algorithms.Scheduling.Model.Shifts
{
    public struct Shift : IShift
    {
        public const int MinutesPerDay = 24*60;

        #region Constructors

        /// <summary>
        /// Use this constructor if you only need 1 shift. The start and end times
        /// will be set to 0 and 24*60 which is the number of minutes in a 24 hour
        /// period.
        /// </summary>
        /// <param name="name">Unique shift name</param>
        public Shift(string name) : this(name, 0, MinutesPerDay)
        {
        }

        /// <summary>
        /// Use this constructor to specify the start and end times of a shift
        /// </summary>
        /// <param name="name">Unique shift name</param>
        /// <param name="startTime">Start time in minutes from 0 (start of day)</param>
        /// <param name="endTime">End time in minutes from 0 (start of day) Cannot exceed 24 * 60</param>
        public Shift(string name, int startTime, int endTime) : this(-1, name, startTime, endTime)
        {
        }

        private Shift(int id, string name, int startTime, int endTime) : this()
        {
            if (id == -1)
            {
                id = this.NextId();
            }
            ID = id;
            Name = name;
            StartTime = startTime;
            EndTime = endTime;
            InstanceCount = 0;

            Workers = new List<IWorker>();
            Tools = new List<ITool>();

            Breaks = new List<ITimeSegment>();
        }

        #endregion END Constructors

        public int ID { get; private set; }
        public string Name { get; private set; }
        public int StartTime { get; private set; }
        public int EndTime { get; private set; }

        public List<IWorker> Workers { get; private set; }
        public List<ITool> Tools { get; private set; }

        public List<ITimeSegment> Breaks { get; private set; }

        public int InstanceCount { get; private set; } 

        public int NextStart()
        {
            return StartTime + InstanceCount++*(MinutesPerDay);
        }
    }
}