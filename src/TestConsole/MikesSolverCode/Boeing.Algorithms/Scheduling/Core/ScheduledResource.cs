using Boeing.Algorithms.Scheduling.Model.Jobs;
using Boeing.Algorithms.Scheduling.Model.Resources.Tools;
using Boeing.Algorithms.Scheduling.Model.Resources.Workers;
using Boeing.Algorithms.Scheduling.Model.Resources.Zones;

namespace Boeing.Algorithms.Scheduling.Core
{
    public interface IScheduledResource<out TResource> : IEventable
    {
        IJob Job { get; }
        TResource Resource { get; }
    }

    public abstract class ScheduledResource<TResource> : IScheduledResource<TResource>
    {
        protected ScheduledResource(TResource resource, int start, int end, IJob job)
        {
            Resource = resource;
            Start = start;
            End = end;
            Job = job;
        }

        public TResource Resource { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        public IJob Job { get; private set; }
    }

    public interface IScheduledWorker : IScheduledResource<IWorker>
    {
    }

    public class ScheduledWorker : ScheduledResource<IWorker>, IScheduledWorker
    {
        public ScheduledWorker(IWorker resource, int start, int end, IJob job)
            : base(resource, start, end, job)
        {
        }
    }

    public interface IScheduledTool : IScheduledResource<IToolInstance>
    {
    }

    public class ScheduledTool : ScheduledResource<IToolInstance>, IScheduledTool
    {
        public ScheduledTool(IToolInstance resource, int start, int end, IJob job)
            : base(resource, start, end, job)
        {
        }
    }

    public interface IScheduledZone : IScheduledResource<IZone>
    {
    }

    public class ScheduledZone : ScheduledResource<IZone>, IScheduledZone
    {
        public ScheduledZone(IZone resource, int start, int end, IJob job)
            : base(resource, start, end, job)
        {
        }
    }
}