using Boeing.Algorithms.Core.Model;

namespace Boeing.Algorithms.Scheduling.Model.Shifts
{
    public class ShiftFactory : EntityGenerator<IShift>
    {
        public override IShift Create(params object[] args)
        {
            string name = "";
            int start = 0;
            int end = 24;

            if (args.Length == 3)
            {
                name = (string) args[0];
                start = (int) args[1];
                end = (int) args[2];
            }
            return new Shift(name, start, end);
        }
    }
}