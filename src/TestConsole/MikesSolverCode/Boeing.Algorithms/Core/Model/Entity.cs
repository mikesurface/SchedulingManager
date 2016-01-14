namespace Boeing.Algorithms.Core.Model
{
    public abstract class Entity<TId> : IEntity<TId>
    {
        protected Entity(TId id)
        {
            ID = id;
        }

        public TId ID { get; protected set; }
    }
}