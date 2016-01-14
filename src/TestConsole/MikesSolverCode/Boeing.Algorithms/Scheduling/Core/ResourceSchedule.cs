using System.Collections.Generic;
using Boeing.Algorithms.Scheduling.Model.Jobs;
using Boeing.Algorithms.Scheduling.Model.Resources.Tools;
using Boeing.Algorithms.Scheduling.Model.Resources.Workers;

namespace Boeing.Algorithms.Scheduling.Core
{
    public interface IResourceSchedule<TScheduledResource, in TResource>
    {
        HashSet<TScheduledResource> ResourceSchedules { get; }
        bool AddScheduledItem(TResource worker, int start, int end, IJob job);
    }

    public abstract class ResourceSchedule<TScheduledResource, TResource> : IResourceSchedule<TScheduledResource, TResource>
    {
        public abstract HashSet<TScheduledResource> ResourceSchedules { get; }

        protected bool AddScheduledResource(TScheduledResource scheduledResource)
        {
            return ResourceSchedules.Add(scheduledResource);
        }

        public abstract bool AddScheduledItem(TResource worker, int start, int end, IJob job);
    }

    public class WorkerSchedule : ResourceSchedule<IScheduledWorker, IWorker>
    {
        public WorkerSchedule()
            : this(new HashSet<IScheduledWorker>())
        {
        }

        public WorkerSchedule(HashSet<IScheduledWorker> resourceSchedules)
        {
            _resourceSchedules = resourceSchedules;
        }

        private readonly HashSet<IScheduledWorker> _resourceSchedules;

        public override HashSet<IScheduledWorker> ResourceSchedules
        {
            get { return _resourceSchedules; }
        }

        public override bool AddScheduledItem(IWorker worker, int start, int end, IJob job)
        {
            var scheduledItem = new ScheduledWorker(worker, start, end, job);
            return AddScheduledResource(scheduledItem);
        }
    }

    public class ToolSchedule : ResourceSchedule<IScheduledTool, IToolInstance>
    {
        public ToolSchedule()
            : this(new HashSet<IScheduledTool>())
        {
        }

        public ToolSchedule(HashSet<IScheduledTool> scheduledTools)
        {
            _resourceSchedules = scheduledTools;
        }

        private readonly HashSet<IScheduledTool> _resourceSchedules;

        public override HashSet<IScheduledTool> ResourceSchedules
        {
            get { return _resourceSchedules; }
        }

        public override bool AddScheduledItem(IToolInstance resource, int start, int end, IJob job)
        {
            var scheduledItem = new ScheduledTool(resource, start, end, job);
            return AddScheduledResource(scheduledItem);
        }
    }
}