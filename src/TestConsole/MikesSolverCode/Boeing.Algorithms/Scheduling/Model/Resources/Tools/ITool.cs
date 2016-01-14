using System.Collections.Generic;
using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Model.Shifts;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Tools
{
    public interface ITool : IEntity<int>
    {
        string ToolType { get; }
        IShift Shift { get; }
        IEnumerable<IToolInstance> ToolInstances { get; }
    }
}