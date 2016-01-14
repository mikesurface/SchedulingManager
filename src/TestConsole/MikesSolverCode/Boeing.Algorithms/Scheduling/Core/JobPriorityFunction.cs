using Boeing.Algorithms.Scheduling.Model.Jobs;

namespace Boeing.Algorithms.Scheduling.Core
{
    public abstract class JobPriorityFunction : IPriorityFunction<IJob, double>
    {
        public abstract double CalculatePriority(IJob input);
    }
}