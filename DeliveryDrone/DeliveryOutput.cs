using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class DeliveryOutput
    {
        public List<Position> DeliveryLocations { get; }
        public string DroneId { get; }

        public DeliveryOutput(List<Position> locations, string droneId)
        {
            DeliveryLocations = locations;
            this.DroneId = droneId;
        }
    }

    public static class DeliveryOutputExtensions
    {
        public static string GetContent(this DeliveryOutput data)
        {
            var builder = new StringBuilder();
            builder.AppendLine("== Reporte de entregas ==");
            foreach(var position in data.DeliveryLocations)
                builder.AppendLine($"({position.PosX}, {position.PosY}) dirección {position.Direction}");
            return builder.ToString();
        }
    }
}
