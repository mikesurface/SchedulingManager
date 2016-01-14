namespace Boeing.Algorithms.Scheduling.Core
{
    public interface IPriorityFunction<in TIn, out TOut>
    {
        TOut CalculatePriority(TIn input);
    }
}