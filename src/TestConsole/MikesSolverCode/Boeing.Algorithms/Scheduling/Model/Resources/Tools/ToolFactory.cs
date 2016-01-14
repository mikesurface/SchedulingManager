using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Shifts;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Tools
{
    public class ToolFactory : EntityGenerator<ITool>
    {
        public override ITool Create(params object[] args)
        {
            var defaultToolType = ToolTypes.Autoclave;
            IShift shift = null;

            if (args.Length == 1)
            {
                defaultToolType = (string) args[0];
            }
            else if (args.Length == 2)
            {
                defaultToolType = (string) args[0];
                shift = (IShift) args[1];
            }

            return new Tool(defaultToolType, shift);
        }
    }
}