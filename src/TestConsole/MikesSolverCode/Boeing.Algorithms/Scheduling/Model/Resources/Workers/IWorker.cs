using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Shifts;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Workers
{
    public interface IWorker : IEntity<int>
    {
        ISkill Skill { get; }
        IShift Shift { get; }
    }
}