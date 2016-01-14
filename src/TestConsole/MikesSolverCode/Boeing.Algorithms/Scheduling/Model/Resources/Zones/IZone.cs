using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Core;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Zones
{
    public interface IZone : IEntity<int>
    {
        string Name { get; }
        int MaxOccupants { get; }
        int AvailableSpots { get; }
        int OccupySpot(InWorkEvent item);
        bool WorkCompleted(InWorkEvent item);
    }
}