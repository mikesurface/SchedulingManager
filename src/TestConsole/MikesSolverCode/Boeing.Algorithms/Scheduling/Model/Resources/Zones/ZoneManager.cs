using System;
using System.Collections.Generic;
using Boeing.Algorithms.Scheduling.Model.Jobs;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Zones
{
    public class ZoneManager
    {
        public ZoneManager(ZoneConverter zoneConverter)
        {
            _zoneConverter = zoneConverter;
        }

        private readonly ZoneConverter _zoneConverter;

        private Dictionary<IZone, Dictionary<int, bool>> _zoneAvailability;
        private Dictionary<IJob, Tuple<IZone, int>> _spots;

        public void ProcessZones(IEnumerable<DataSetServices.Data.Modeling.Resources.IZone> zones)
        {
            _zoneAvailability = new Dictionary<IZone, Dictionary<int, bool>>();
            _spots = new Dictionary<IJob, Tuple<IZone, int>>();

            foreach (var zone in zones)
            {
                var converted = CreateZoneFromZone(zone);
                _zoneAvailability.Add(converted, new Dictionary<int, bool>());
                for (int i = 0; i < zone.MaxOccupants; i++)
                {
                    _zoneAvailability[converted].Add(i, true);
                }
            }
        }

        private IZone CreateZoneFromZone(DataSetServices.Data.Modeling.Resources.IZone zone)
        {
            return _zoneConverter.Convert(zone);
        }

        public IEnumerable<IZone> GetZones()
        {
            return _zoneConverter.Instances.Values;
        }

        public IZone GetZone(string name)
        {
            return _zoneConverter.NamedInstances[name];
        }

        private int GetNextAvailableSpot(IZone zone, bool take = false)
        {
            foreach (var spot in _zoneAvailability[zone])
            {
                if (spot.Value)
                {
                    if (take)
                    {
                        _zoneAvailability[zone][spot.Key] = false;
                    }
                    return spot.Key;
                }
            }
            return -1;
        }

        public bool CanWorkJob(IJob job)
        {
            var spot = GetNextAvailableSpot(job.Location);
            return spot != -1;
        }

        public bool ScheduleJob(IJob job)
        {
            var spot = GetNextAvailableSpot(job.Location, true);
            return spot != -1;
        }

        public bool CompleteJob(IJob job)
        {
            if (!_spots.ContainsKey(job)) return false;
            var zoneSpot = _spots[job];
            _zoneAvailability[zoneSpot.Item1][zoneSpot.Item2] = false;
            return true;
        }
    }
}