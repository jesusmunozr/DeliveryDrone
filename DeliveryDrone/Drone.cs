using Infrastructure;
using Infrastructure.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class Drone : ITransport
    {
        private const short CAPACITY = 3;
        private const short MAXIMUM_DISTANCE = 10;
        private Delivery delivery;
        public string Id { get; private set; }

        public Location CurrentLocation { get; private set; }

        public Drone()
        {
            CurrentLocation = new Location();
        }

        public Task<DeliveryOutput> DeliverAsync(IFileManager fileManager, Delivery deliveryInfo)
        {
            Id = deliveryInfo.DroneId;
            delivery = deliveryInfo;

            if (delivery.Routes.Count > CAPACITY)
                throw new TransportException("Drone capacity exceeded.", Id);

            return Task.Run(() =>
            {
                var locations = new List<Location>();
                foreach (var route in delivery.Routes)
                {
                    while (route.MoveNext())
                        Move(route.Current);
                    locations.Add((Location)CurrentLocation.Clone());
                }

                var result = new DeliveryOutput(locations, Id, fileManager);

                return result;
            });
        }

        private void Move(char step)
        {
            CurrentLocation.NextStep(step);

            if (!CurrentLocation.IsValid(MAXIMUM_DISTANCE))
                throw new TransportException("Drone out of range.", Id);
        }
    }
}
