namespace Boeing.Algorithms.Scheduling.Core
{
    public class Event<T> where T : IEventable
    {
        public Event(T item)
        {
            Owner = item;
            Start = item.Start;
            End = item.End;
        }

        public T Owner { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
    }
}