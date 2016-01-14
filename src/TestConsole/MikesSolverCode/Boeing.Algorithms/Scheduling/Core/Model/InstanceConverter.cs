using System.Collections.Concurrent;
using System.Collections.Generic;
using Boeing.Algorithms.Core.Model;

namespace Boeing.Algorithms.Scheduling.Core.Model
{
    public abstract class InstanceConverter<TInput, TOutput> where TOutput : IEntity<int>
    {
        protected InstanceConverter(EntityGenerator<TOutput> factory)
        {
            Factory = factory;
            Instances = new ConcurrentDictionary<int, TOutput>();
            NamedInstances = new ConcurrentDictionary<string, TOutput>();
            InstancesMap = new ConcurrentDictionary<TInput, IEnumerable<TOutput>>();
        }

        protected EntityGenerator<TOutput> Factory { get; private set; }
        public ConcurrentDictionary<int, TOutput> Instances { get; private set; }
        public ConcurrentDictionary<string, TOutput> NamedInstances { get; private set; }
        public ConcurrentDictionary<TInput, IEnumerable<TOutput>> InstancesMap { get; private set; }

        public abstract IEnumerable<TOutput> Convert(TInput input);
        public abstract TInput ConvertBack(IEnumerable<TOutput> output);
    }
}