using Boeing.Algorithms.Scheduling.Model.Jobs;

namespace Boeing.Algorithms.Scheduling.Core
{
    /// <summary>
    /// Calculates a job's priority based on STR:
    /// Slack Time Remaining
    /// </summary>
    public class SlackTimeRemainingJobPriorityFunction : JobPriorityFunction
    {
        public override double CalculatePriority(IJob input)
        {
            return input.Slack;
        }
    }
}