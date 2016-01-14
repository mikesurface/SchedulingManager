namespace Boeing.Algorithms.Scheduling.Core
{
    public interface ITimeSegment
    {
        int StartTime { get; }
        int EndTime { get; }
    }
}