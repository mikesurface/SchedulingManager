using System.Collections.Generic;
using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Core;
using Boeing.Algorithms.Scheduling.Model.Resources.Tools;
using Boeing.Algorithms.Scheduling.Model.Resources.Workers;

namespace Boeing.Algorithms.Scheduling.Model.Shifts
{
    public interface IShift : IEntity<int>, ITimeSegment
    {
        string Name { get; }
        List<IWorker> Workers { get; }
        List<ITool> Tools { get; }
        List<ITimeSegment> Breaks { get; }
        int NextStart();
    }
}