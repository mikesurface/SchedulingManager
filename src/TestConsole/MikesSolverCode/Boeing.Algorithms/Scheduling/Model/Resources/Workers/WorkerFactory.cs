using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Shifts;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Workers
{
    public class WorkerFactory : EntityGenerator<IWorker>
    {
        public override IWorker Create(params object[] args)
        {
            ISkill skill = null;
            IShift shift = null;

            if (args.Length == 1)
            {
                skill = (ISkill) args[0];
            }
            else if (args.Length == 2)
            {
                skill = (ISkill) args[0];
                shift = (IShift) args[1];
            }

            return new Worker(skill, shift);
        }
    }
}