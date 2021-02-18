using System;

namespace Infrastructure.Exceptions
{
    public class DroneException : Exception
    {
        public string DroneId { get; }

        public DroneException(string message, string droneId)
            : base(message)
        {
            DroneId = droneId;
        }
    }
}
