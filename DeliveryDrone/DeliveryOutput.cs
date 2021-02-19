using Infrastructure;
using Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class DeliveryOutput
    {
        public List<Location> DeliveryLocations { get; }
        public string DroneId { get; }
        public bool Fail { get; }
        public ExceptionBase FailureException { get; }

        private readonly IFileManager fileManager;

        public DeliveryOutput(List<Location> locations, string droneId, IFileManager fileManager)
        {
            DeliveryLocations = locations;
            DroneId = droneId;
            Fail = false;
            this.fileManager = fileManager;
            FailureException = null;
        }

        public DeliveryOutput(ExceptionBase exception, IFileManager fileManager)
        {
            DeliveryLocations = null;
            DroneId = exception.DroneId;
            Fail = true;
            FailureException = exception;
            this.fileManager = fileManager;
        }

        public async Task SaveAsync()
        {
            var builder = new StringBuilder();
            builder.AppendLine("== Reporte de entregas ==");

            if (!Fail)
            {
                foreach (var location in DeliveryLocations)
                    builder.AppendLine($"({location.PosX}, {location.PosY}) dirección {location.Orientation.GetName()}");
            }
            else
            {
                builder.AppendLine($"Error: {FailureException.Message}");
            }

            await fileManager.CreateOutputFileAsync($"out{DroneId}.txt", builder.ToString());
        }
    }
}
