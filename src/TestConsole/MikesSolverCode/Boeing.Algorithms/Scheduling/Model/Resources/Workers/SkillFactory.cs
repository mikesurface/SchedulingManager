using Boeing.Algorithms.Core.Model;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Workers
{
    public class SkillFactory : EntityGenerator<ISkill>
    {
        public override ISkill Create(params object[] args)
        {
            var skillName = "";

            if (args.Length == 1)
            {
                skillName = (string) args[0];
            }

            return new Skill(skillName);
        }
    }
}