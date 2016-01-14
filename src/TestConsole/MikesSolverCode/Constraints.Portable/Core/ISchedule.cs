using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSetServices.Data.Core.BaseTypes;
using DataSetServices.Data.Domain;
using DataSetServices.Data.Modeling.Core;

namespace DataSetServices.Data.Analytics.ConstraintSolver.Core
{
    public interface IItemSchedule<out TItem> where TItem : IDataItem
    {
        TItem ScheduledItem { get; }
        TimeFrame ScheduledStart { get; set; }
        TimeFrame ScheduledFinish { get; set; }
    }

    public abstract class ItemSchedule<TItem> : IItemSchedule<TItem> where TItem : IDataItem
    {
        protected ItemSchedule(TItem item)
        {
            ScheduledItem = item;
        }

        public TItem ScheduledItem { get; private set; }

        public TimeFrame ScheduledStart { get; set; }
        public TimeFrame ScheduledFinish { get; set; }
    }

    public class ResourceSchedule : ItemSchedule<IResource>
    {
        public ResourceSchedule(IResource resource)
            : base(resource)
        {
        }
    }

    public interface IProjectSchedule<TScheduledItem, TItem>
        where TScheduledItem : IItemSchedule<TItem>
        where TItem : IDataItem
    {
        IDictionary<int, ConcurrentDictionary<TItem, TScheduledItem>> ItemSchedules { get; }
        void AddOrUpdateItemSchedule(int interval, TScheduledItem itemSchedule);
    }

    public abstract class ProjectSchedule<TScheduledItem, TItem> : IProjectSchedule<TScheduledItem, TItem>
        where TScheduledItem : IItemSchedule<TItem>
        where TItem : IDataItem
    {
        public abstract IDictionary<int, ConcurrentDictionary<TItem, TScheduledItem>> ItemSchedules { get; }

        public void AddOrUpdateItemSchedule(int interval, TScheduledItem itemSchedule)
        {
            ConcurrentDictionary<TItem, TScheduledItem> intervalItems;
            if (!ItemSchedules.TryGetValue(interval, out intervalItems))
            {
                intervalItems = new ConcurrentDictionary<TItem, TScheduledItem>();
                ItemSchedules.Add(interval, intervalItems);
            }
            intervalItems.GetOrAdd(itemSchedule.ScheduledItem, itemSchedule);
        }

        public int FindItemInterval(TItem item)
        {
            foreach (var kvp in ItemSchedules)
            {
                if (kvp.Value.ContainsKey(item))
                {
                    return kvp.Key;
                }
            }
            return -1;
        }
    }

    public class ResourceProjectSchedule : ProjectSchedule<ResourceSchedule, IResource>
    {
        public ResourceProjectSchedule()
        {
            _itemSchedules = new ConcurrentDictionary<int, ConcurrentDictionary<IResource, ResourceSchedule>>();
        }

        private readonly IDictionary<int, ConcurrentDictionary<IResource, ResourceSchedule>> _itemSchedules;
        public override IDictionary<int, ConcurrentDictionary<IResource, ResourceSchedule>> ItemSchedules
        {
            get { return _itemSchedules; }
        }
    }

    public class ActivityProjectSchedule : ProjectSchedule<IJobSchedule, IActivity>
    {
        public ActivityProjectSchedule()
        {
            _itemSchedules = new ConcurrentDictionary<int, ConcurrentDictionary<IActivity, IJobSchedule>>();
        }

        private readonly IDictionary<int, ConcurrentDictionary<IActivity, IJobSchedule>> _itemSchedules;
        public override IDictionary<int, ConcurrentDictionary<IActivity, IJobSchedule>> ItemSchedules
        {
            get { return _itemSchedules; }
        }

        public IEnumerable<IJobSchedule> ActivitySchedules
        {
            get { return ItemSchedules.Values.SelectMany(x => x.Values); }
        }

        public int TotalFlow
        {
            get { return ItemSchedules.Sum(xx => ItemSchedules[xx.Key].Values.Max(x => (int)x.ScheduledFinish.InMinutes() - (int)x.ScheduledStart.InMinutes())); }
        }

        public double TotalLaborHours
        {
            get { return ActivitySchedules.Sum(x => (x.ScheduledFinish - x.ScheduledStart).InMinutes()) / 60; }
        }
    }
}
