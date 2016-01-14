using System.Collections.Generic;
using Boeing.Algorithms.Scheduling.Model.Jobs;

namespace Boeing.Algorithms.Scheduling.Core
{
    public interface IJobProcessor
    {
        void Enqueue(IJob job);
        IJob Dequeue(int iteration);
        IJob Peek();
        void Update(IJob job);
        void Complete(IJob job, int iteration);

        bool Empty { get; }
        bool NotEmpty { get; }
        
        List<IJob> ReceivedJobs { get; }
        List<IJob> ProcessingJobs { get; }
        Dictionary<int, IJob> ProcessedJobs { get; }
    }
}