using Boeing.Algorithms.Core.Model;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Zones
{
    public class ZoneFactory : EntityGenerator<IZone>
    {
        public override IZone Create(params object[] args)
        {
            var maxOccupants = 3;
            var name = "location";

            if (args.Length == 1)
            {
                maxOccupants = (int) args[0];
            }
            else if (args.Length == 2)
            {
                name = (string) args[0];
                maxOccupants = (int) args[1];
            }

            return new Zone(name, maxOccupants);
        }
    }
}