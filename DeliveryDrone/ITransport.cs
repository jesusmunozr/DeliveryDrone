using Infrastructure;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public interface ITransport
    {
        Location CurrentLocation { get; }

        //Task<DeliveryOutput> DeliverAsync(IFileManager fileManager);
        Task<DeliveryOutput> DeliverAsync(IFileManager fileManager, Delivery deliveryInfo);
    }
}
