namespace Boeing.Algorithms.Core.Model
{
    public abstract class EntityGenerator<TEntity> where TEntity : IEntity<int>
    {
        private readonly object _idLock = new object();
        private int _lastGeneratedId = 0;

        protected int NextId()
        {
            lock (_idLock)
            {
                return ++_lastGeneratedId;
            }
        }

        public abstract TEntity Create(params object[] args);
    }
}