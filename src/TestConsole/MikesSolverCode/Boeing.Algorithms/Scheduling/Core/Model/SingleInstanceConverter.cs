using System.Collections.Concurrent;
using System.Linq;
using Boeing.Algorithms.Core.Model;

namespace Boeing.Algorithms.Scheduling.Core.Model
{
    public abstract class SingleInstanceConverter<TInput, TOutput> where TOutput : IEntity<int>
    {
        protected SingleInstanceConverter(EntityGenerator<TOutput> factory)
        {
            Factory = factory;
            Instances = new ConcurrentDictionary<int, TOutput>();
            NamedInstances = new ConcurrentDictionary<string, TOutput>();
            InstancesMap = new ConcurrentDictionary<TInput, TOutput>();
        }

        protected EntityGenerator<TOutput> Factory { get; private set; }
        public ConcurrentDictionary<int, TOutput> Instances { get; private set; }
        public ConcurrentDictionary<string, TOutput> NamedInstances { get; private set; }
        public ConcurrentDictionary<TInput, TOutput> InstancesMap { get; private set; }

        public abstract TOutput Convert(TInput input);

        public TInput ConvertBack(TOutput output)
        {
            return InstancesMap.First(x => x.Value.Equals(output)).Key;
        }
    }
}