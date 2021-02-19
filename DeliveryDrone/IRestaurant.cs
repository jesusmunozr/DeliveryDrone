using System.Threading.Tasks;

namespace DeliveryDrone
{
    public interface IRestaurant
    {
        Task DispatchLunchesAsync(string inputFolder, TransportTypes transport);
    }
}
