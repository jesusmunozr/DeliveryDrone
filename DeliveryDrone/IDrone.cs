using Infrastructure;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public interface IDrone
    {
        Location CurrentLocation { get; }

        Task<DeliveryOutput> StartDelivery(IFileManager fileManager);
    }
}
