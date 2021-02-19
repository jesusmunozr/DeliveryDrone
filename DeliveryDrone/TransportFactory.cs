using System;

namespace DeliveryDrone
{
    public class TransportFactory : ITransportFactory
    {
        public ITransport CreateTransport(TransportTypes transportType)
        {
            ITransport transport = transportType switch
            {
                TransportTypes.LightDrone => new Drone(),
                TransportTypes.StrongDrone => new StrongDrone(),
                TransportTypes.AutonomousVehicle => new AutonomousVehicle(),
                _ => throw new NotImplementedException()
            };
            return transport;
        }
    }
}
