using System.Collections.Generic;
using System.Text;
using Boeing.Algorithms.Core.Model;

namespace Boeing.Algorithms.Scheduling.Model.Jobs
{
    public class JobOld : Entity<int>
    {
        public JobOld(
            int id,
            int duration,
            string locationName,
            string requiredSkillType,
            string requiredToolType) : base(id)
        {
            FirstPossibleStart = -1;

            Duration = duration;
            LocationName = locationName;
            RequiredSkillType = requiredSkillType;
            RequiredToolType = requiredToolType;

            Predecessors = new HashSet<JobOld>();
            Successors = new HashSet<JobOld>();
        }

        public int Duration { get; set; }
        public int FirstPossibleStart { get; set; }
        public string LocationName { get; set; }
        public string RequiredSkillType { get; set; }
        public string RequiredToolType { get; set; }
        public HashSet<JobOld> Predecessors { get; private set; }
        public HashSet<JobOld> Successors { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(
                "ID: {0} Duration: {1} locationZone: {2} RequiredSkill: {3} RequiredTool: {4}", 
                ID,
                Duration,
                LocationName,
                RequiredSkillType,
                RequiredToolType);

            sb.Append(" Predecessors: ");
            if (Predecessors.Count > 0)
            {
                foreach (var predecessor in Predecessors)
                {
                    sb.AppendFormat("{0},", predecessor.ID);
                }
                sb.Remove(sb.Length - 1, 1);
            }
            else
            {
                sb.Append("0");
            }

            sb.Append(" Successors: ");
            if (Successors.Count > 0)
            {
                foreach (var successor in Successors)
                {
                    sb.AppendFormat("{0},", successor.ID);
                }
                sb.Remove(sb.Length - 1, 1);
            }
            else
            {
                sb.Append("0");
            }

            return sb.ToString();
        }
    }
}