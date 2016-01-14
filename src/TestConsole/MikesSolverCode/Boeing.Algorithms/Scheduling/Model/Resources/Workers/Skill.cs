using Boeing.Algorithms.Core.Model;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Workers
{
    public struct Skill : ISkill
    {
        #region Constructors

        public Skill(string skillType) : this(-1, skillType)
        {
        }

        public Skill(int id, string skillType) : this()
        {
            if (id == -1)
            {
                id = this.NextId();
            }
            ID = id;
            SkillType = skillType;
        }

        #endregion END Constructors

        public int ID { get; private set; }
        public string SkillType { get; private set; }

        public override string ToString()
        {
            return string.Format("Skill: {0}-{1}", ID, SkillType);
        }
    }
}