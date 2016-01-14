using System;
using System.Collections.Generic;
using System.Security.Policy;
using Boeing.Algorithms.Scheduling.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Shifts;
using DataSetServices.Data.Domain;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Workers
{
    public class LaborConverter : InstanceConverter<AvailableLaborLink, IWorker>
    {
        public LaborConverter(
            WorkerFactory factory,
            ShiftConverter shiftConverter,
            SkillConverter skillConverter) : base(factory)
        {
            _shiftConverter = shiftConverter;
            _skillConverter = skillConverter;
            WorkerTypes = new Dictionary<ISkill, Dictionary<int, IWorker>>();
        }

        private readonly ShiftConverter _shiftConverter;
        private readonly SkillConverter _skillConverter;

        public Dictionary<ISkill, Dictionary<int, IWorker>> WorkerTypes { get; private set; }

        public override IEnumerable<IWorker> Convert(AvailableLaborLink input)
        {
            if (!InstancesMap.ContainsKey(input))
            {
                var labor = input.Labor;
                if (!_skillConverter.NamedInstances.ContainsKey(labor.Name))
                {
                    _skillConverter.Convert(labor);
                }
                ISkill skill = _skillConverter.InstancesMap[labor];

                if (!WorkerTypes.ContainsKey(skill))
                {
                    WorkerTypes.Add(skill, new Dictionary<int, IWorker>());
                }

                if (!_shiftConverter.NamedInstances.ContainsKey(input.Shift.Name))
                {
                    _shiftConverter.Convert(input.Shift);
                }
                IShift shift = _shiftConverter.InstancesMap[input.Shift];

                var workers = new HashSet<IWorker>();
                for (int i = 0; i < input.AvailableQuantity; i++)
                {
                    var worker = Factory.Create(skill, shift);
                    workers.Add(worker);
                    WorkerTypes[skill].Add(worker.ID, worker);
                }

                InstancesMap.GetOrAdd(input, workers);
            }
            else
            {
                string test = "do we really already have it?";
            }

            return InstancesMap[input];
        }

        public override AvailableLaborLink ConvertBack(IEnumerable<IWorker> output)
        {
            throw new NotImplementedException();
        }
    }
}