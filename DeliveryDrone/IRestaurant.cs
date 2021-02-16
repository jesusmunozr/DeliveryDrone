using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public interface IRestaurant
    {
        Task DispatchDrones(string inputFolder);
    }
}
