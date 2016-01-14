using Boeing.Algorithms.Core.Model;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Workers
{
    public interface ISkill : IEntity<int>
    {
        string SkillType { get; }
    }
}