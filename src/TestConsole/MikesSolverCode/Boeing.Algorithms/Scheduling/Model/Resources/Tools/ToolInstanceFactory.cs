using Boeing.Algorithms.Core.Model;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Tools
{
    public class ToolInstanceFactory : EntityGenerator<IToolInstance>
    {
        public override IToolInstance Create(params object[] args)
        {
            ITool tool = null;
            if (args.Length == 1)
            {
                tool = (ITool) args[0];
            }
            return new ToolInstance(tool);
        }
    }
}