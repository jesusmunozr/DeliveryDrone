using Infrastructure;
using System;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class StrongDrone : ITransport
    {
        public Location CurrentLocation => throw new NotImplementedException();

        public Task<DeliveryOutput> DeliverAsync(IFileManager fileManager, Delivery deliveryInfo)
        {
            throw new NotImplementedException();
        }
    }
}
