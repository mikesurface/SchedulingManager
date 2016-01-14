using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Resources.Tools;
using Boeing.Algorithms.Scheduling.Model.Resources.Workers;
using Boeing.Algorithms.Scheduling.Model.Resources.Zones;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;

namespace Boeing.Algorithms.Scheduling.Model.Jobs
{
    public class JobConverter : SingleInstanceConverter<IActivitySchedule, IJob>
    {
        #region Constructor

        public JobConverter(
            EntityGenerator<IJob> factory,
            SkillConverter skillConverter,
            ToolConverter toolConverter,
            ZoneConverter zoneConverter) : base(factory)
        {
            _skillConverter = skillConverter;
            _toolConverter = toolConverter;
            _zoneConverter = zoneConverter;
        }

        private readonly SkillConverter _skillConverter;
        private readonly ToolConverter _toolConverter;
        private readonly ZoneConverter _zoneConverter;

        #endregion END Constructor

        #region Conversion Logic

        public override IJob Convert(IActivitySchedule input)
        {
            if (!InstancesMap.ContainsKey(input))
            {
                IJob converted = null;
                var duration = (int) input.TargetDuration.AsMinutes;
                var slack = (int) input.Float.AsMinutes;

                IZone zone = null;
                foreach (var zl in input.ScheduledItem.ZoneLinks)
                {
                    if (!_zoneConverter.NamedInstances.ContainsKey(zl.Zone.Name))
                    {
                        _zoneConverter.Convert(zl.Zone);
                    }
                    zone = _zoneConverter.InstancesMap[zl.Zone];
                }
                converted = Factory.Create(input.ScheduledItem.UID, duration, slack, zone);

                // required skill types
                foreach (var ll in input.ScheduledItem.LaborLinks)
                {
                    var labor = ll.Labor;
                    converted.RequiredSkillSet.Add(_skillConverter.NamedInstances[labor.Name]);
                }

                // required tool types
                foreach (var tl in input.ScheduledItem.ToolLinks)
                {
                    var tool = tl.Tool;
                    converted.RequiredToolSet.Add(_toolConverter.NamedInstances[tool.Name]);
                }

                InstancesMap.GetOrAdd(input, converted);
            }
            return InstancesMap[input];
        }

        #endregion END Conversion Logic
    }
}