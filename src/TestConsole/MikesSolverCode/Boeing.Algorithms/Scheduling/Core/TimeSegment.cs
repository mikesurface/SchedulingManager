namespace Boeing.Algorithms.Scheduling.Core
{
    public struct TimeSegment : ITimeSegment
    {
        public TimeSegment(int startTime, int endTime) : this()
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public int StartTime { get; private set; }
        public int EndTime { get; private set; }
    }
}