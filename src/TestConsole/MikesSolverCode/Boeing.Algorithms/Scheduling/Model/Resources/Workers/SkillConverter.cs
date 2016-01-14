using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Core.Model;
using DataSetServices.Data.Modeling.Resources;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Workers
{
    public class SkillConverter : SingleInstanceConverter<ILabor, ISkill>
    {
        public SkillConverter(EntityGenerator<ISkill> factory) : base(factory)
        {
        }

        public override ISkill Convert(ILabor input)
        {
            if (!InstancesMap.ContainsKey(input))
            {
                var converted = Factory.Create(input.Name);
                InstancesMap.GetOrAdd(input, converted);
                NamedInstances.GetOrAdd(input.Name, converted);
            }
            return InstancesMap[input];
        }
    }
}