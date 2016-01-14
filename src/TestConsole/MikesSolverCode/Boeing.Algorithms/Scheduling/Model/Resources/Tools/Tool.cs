using System.Collections.Generic;
using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Shifts;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Tools
{
    public struct Tool : ITool
    {
        #region Constructors

        public Tool(string toolType): this(toolType, null)
        {
        }

        public Tool(string toolType, IShift shift): this(-1, toolType, shift)
        {
        }

        private Tool(int id, string toolType, IShift shift) : this()
        {
            if (id == -1)
            {
                id = this.NextId();
            }
            ID = id;
            ToolType = toolType;
            Shift = shift;
            ToolInstanceKeys = new Dictionary<int, IToolInstance>();
        }

        #endregion END Constructors

        public int ID { get; private set; }
        public string ToolType { get; private set; }
        public IShift Shift { get; private set; }

        public Dictionary<int, IToolInstance> ToolInstanceKeys { get; private set; }

        public IEnumerable<IToolInstance> ToolInstances
        {
            get { return ToolInstanceKeys.Values; }
        }

        public override string ToString()
        {
            return string.Format("ID: {0}, ToolType: {1}, Shift: {2}", ID, ToolType, Shift);
        }
    }
}