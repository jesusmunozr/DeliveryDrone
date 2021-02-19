using System;

namespace Infrastructure.Exceptions
{
    public abstract class ExceptionBase : Exception
    {
        public string DroneId { get; }

        public ExceptionBase(string message, string droneId)
            : base(message)
        {
            DroneId = droneId;
        }
    }
}
