using Boeing.Algorithms.Scheduling.Model.Shifts;

namespace Boeing.Algorithms.Scheduling.Core
{
    public class InWorkEvent : Event<IScheduledJob>
    {
        public InWorkEvent(IShift shift, IScheduledJob item) : base(item)
        {
            Shift = shift;
        }

        public IShift Shift { get; private set; }
    }
}