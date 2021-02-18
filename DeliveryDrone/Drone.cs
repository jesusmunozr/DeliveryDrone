using Infrastructure;
using Infrastructure.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class Drone : IDrone
    {
        private const short CAPACITY = 3;
        private readonly IList<Delivery> deliveries;
        public readonly string Id;

        public Location CurrentLocation { get; private set; }

        public Drone(IList<Delivery> deliveries, string droneId)
        {
            Id = droneId;
            CurrentLocation = new Location();
            this.deliveries = deliveries;
        }

        public Task<DeliveryOutput> StartDelivery(IFileManager fileManager)
        {
            if (deliveries.Count > CAPACITY)
                throw new DroneException("Drone capacity exceeded.", Id);

            return Task.Run(() =>
            {
                var locations = new List<Location>();
                foreach (var delivery in deliveries)
                {
                    var deliveryEnumerator = delivery.GetEnumerator();
                    while (deliveryEnumerator.MoveNext())
                        Move(deliveryEnumerator.Current, Id);
                    locations.Add((Location)CurrentLocation.Clone());
                }

                var result = new DeliveryOutput(locations, Id, fileManager);

                return result;
            });
        }

        private void Move(char step, string droneId)
        {
            CurrentLocation.NextStep(step);

            if (!CurrentLocation.IsValid())
                throw new DroneException("Drone out of range.", droneId);
        }
    }
}
