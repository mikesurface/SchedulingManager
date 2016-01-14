using System;
using System.Collections.Generic;
using System.Linq;
using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Shifts;
using DataSetServices.Data.Domain;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Tools
{
    public class ToolConverter : InstanceConverter<AvailableToolLink, ITool>
    {
        public ToolConverter(EntityGenerator<ITool> factory, ShiftConverter shiftConverter) : base(factory)
        {
            _shiftConverter = shiftConverter;
            _toolInstanceFactory = new ToolInstanceFactory();
            ToolTypes = new Dictionary<ITool, Dictionary<int, IToolInstance>>();
        }

        private readonly ShiftConverter _shiftConverter;
        private readonly ToolInstanceFactory _toolInstanceFactory;

        public Dictionary<ITool, Dictionary<int, IToolInstance>> ToolTypes { get; private set; }

        public override IEnumerable<ITool> Convert(AvailableToolLink input)
        {
            if (!InstancesMap.ContainsKey(input))
            {
                var tool = input.Tool;

                if (!_shiftConverter.NamedInstances.ContainsKey(input.Shift.Name))
                {
                    _shiftConverter.Convert(input.Shift);
                }
                IShift shift = _shiftConverter.InstancesMap[input.Shift];

                var tools = new HashSet<ITool>();
                var converted = Factory.Create(tool.Name, shift);
                tools.Add(converted);
                var instances = new Dictionary<int, IToolInstance>();
                ToolTypes.Add(converted, instances);
                for (int i = 0; i < input.AvailableQuantity; i++)
                {
                    var instance = _toolInstanceFactory.Create(converted);
                    instances.Add(instance.ID, instance);
                }

                InstancesMap.GetOrAdd(input, tools);
                // TODO: this should be a toolInstance converter and a toolDefinition converter
                // TODO: we have skillConverter which is the definition part, and laborConverter which is the instance part
                NamedInstances.GetOrAdd(tool.Name, tools.First());
            }

            return InstancesMap[input];
        }

        public override AvailableToolLink ConvertBack(IEnumerable<ITool> output)
        {
            throw new NotImplementedException();
        }
    }
}