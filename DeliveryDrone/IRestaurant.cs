using System.Threading.Tasks;

namespace DeliveryDrone
{
    public interface IRestaurant
    {
        Task DispatchDronesAsync(string inputFolder);
    }
}
