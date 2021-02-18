using System;

namespace Infrastructure.Exceptions
{
    public class DeliveryPathException : Exception
    {
        public string DroneId { get; }

        public DeliveryPathException(string message, string droneId)
            : base(message)
        {
            DroneId = droneId;
        }
    }
}
