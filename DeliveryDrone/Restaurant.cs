using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class Restaurant
    {
        private const short dronCapacity = 20;
        private readonly IFileManager fileManager;

        public Restaurant(IFileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        public async Task DispatchDeliveries(string inputFolder)
        {
            var inputFiles = fileManager.ListInputFiles(inputFolder);

            foreach(var delivery in inputFiles)
            {
                var inputData = await fileManager.ReadDeliveryFileAsync(delivery);

                var drone = new Drone(inputData);
                drone.OnDeliveryFinished += Drone_OnDeliveryFinished;
                await drone.Fly();
            }
        }

        private void Drone_OnDeliveryFinished(object sender, Position e)
        {
            
        }
    }
}
