using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Core.Model;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Zones
{
    public class ZoneConverter : SingleInstanceConverter<DataSetServices.Data.Modeling.Resources.IZone, IZone>
    {
        public ZoneConverter(EntityGenerator<IZone> factory) : base(factory)
        {
        }

        public override IZone Convert(DataSetServices.Data.Modeling.Resources.IZone input)
        {
            if (!InstancesMap.ContainsKey(input))
            {
                var converted = Factory.Create(input.Name, input.MaxOccupants);
                return InstancesMap.GetOrAdd(input, converted);
            }
            return InstancesMap[input];
        }
    }
}