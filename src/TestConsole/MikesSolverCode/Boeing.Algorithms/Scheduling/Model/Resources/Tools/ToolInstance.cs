using Boeing.Algorithms.Core.Model;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Tools
{
    public struct ToolInstance : IToolInstance
    {
        #region Constructors

        public ToolInstance(ITool definition) : this(-1, definition)
        {
        }

        private ToolInstance(int id, ITool definition) : this()
        {
            if (id == -1)
            {
                id = this.NextId();
            }
            ID = id;
            Definition = definition;
        }

        #endregion END Constructors

        public int ID { get; private set; }
        public ITool Definition { get; private set; }
    }
}