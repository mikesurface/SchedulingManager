using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Shifts;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Workers
{
    public struct Worker : IWorker
    {
        #region Constructors

        public Worker(ISkill skill) : this(skill, null)
        {
        }

        public Worker(ISkill skill, IShift shift) : this(-1, skill, shift)
        {
        }

        private Worker(int id, ISkill skill, IShift shift) : this()
        {
            if (id == -1)
            {
                id = this.NextId();
            }
            ID = id;
            Skill = skill;
            Shift = shift;
        }

        #endregion END Constructors

        public int ID { get; private set; }
        public ISkill Skill { get; private set; }
        public IShift Shift { get; private set; }

        public override string ToString()
        {
            return string.Format("ID: {0}, Skill: {1}, Shift: {2}", ID, Skill, Shift);
        }
    }
}