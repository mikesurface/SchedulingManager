using Boeing.Algorithms.Core.Model;
using Boeing.Algorithms.Scheduling.Core;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Zones
{
    public struct Zone : IZone
    {
        #region Constructors

        public Zone(string name, int maxOccupants) : this(-1, name, maxOccupants)
        {
        }

        private Zone(int id, string name, int maxOccupants) : this()
        {
            if (id == -1)
            {
                id = this.NextId();
            }
            ID = id;
            Name = name;
            MaxOccupants = maxOccupants;
            _availableSpots = MaxOccupants;
            _occupiedSpots = new InWorkEvent[MaxOccupants];
        }

        #endregion END Constructors

        public int ID { get; private set; }
        public string Name { get; private set; }
        public int MaxOccupants { get; private set; }

        private int _availableSpots;
        public int AvailableSpots
        {
            get { return _availableSpots; }
        }

        public int OccupySpot(InWorkEvent item)
        {
            if (_availableSpots > 0)
            {
                for (int i = 0; i < MaxOccupants; i++)
                {
                    if (_occupiedSpots[i] == null)
                    {
                        _occupiedSpots[i] = item;
                        return _availableSpots--;
                    }
                }
            }

            return -1;
        }

        public bool WorkCompleted(InWorkEvent item)
        {
            InWorkEvent found = null;

            for (int i = 0; i < MaxOccupants; i++)
            {
                if (_occupiedSpots[i] != null && _occupiedSpots[i].Owner.Job.ID == item.Owner.Job.ID)
                {
                    found = _occupiedSpots[i];
                    _occupiedSpots[i] = null;
                    _availableSpots++;
                    break;
                }
            }

            return found != null;
        }

        private readonly InWorkEvent[] _occupiedSpots;

        public override string ToString()
        {
            return string.Format("ID: {0}, MaxOccupants: {1}", ID, MaxOccupants);
        }
    }
}