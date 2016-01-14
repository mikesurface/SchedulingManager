namespace Boeing.Algorithms.Scheduling.Core
{
    public struct TimePoint
    {
        public TimePoint(int time) : this()
        {
            Time = time;
        }

        public int Time { get; set; }
    }
}