using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Core.Model;

namespace Boeing.Algorithms.Scheduling.Model.Shifts
{
    public class ShiftConverter : SingleInstanceConverter<DataSetServices.Data.Modeling.Core.IShift, IShift>
    {
        public ShiftConverter(EntityGenerator<IShift> factory) : base(factory)
        {
        }

        public override IShift Convert(DataSetServices.Data.Modeling.Core.IShift input)
        {
            if (!InstancesMap.ContainsKey(input))
            {
                var start = (int) input.StartTime.AsMinutes;
                var end = (int) input.EndTime.AsMinutes;
                var converted = Factory.Create(input.Name, start, end);
                NamedInstances.GetOrAdd(input.Name, converted);
                return InstancesMap.GetOrAdd(input, converted);
            }
            return InstancesMap[input];
        }
    }
}