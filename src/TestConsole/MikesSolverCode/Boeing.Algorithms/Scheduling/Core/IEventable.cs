namespace Boeing.Algorithms.Scheduling.Core
{
    public interface IEventable
    {
        int Start { get; }
        int End { get; }
    }
}