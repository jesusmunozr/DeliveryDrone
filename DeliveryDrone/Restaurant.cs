using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class Restaurant : IRestaurant
    {
        private const short dronCapacity = 20;
        private readonly IFileManager fileManager;

        public Restaurant(IFileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        public async Task DispatchDrones(string inputFolder)
        {
            var inputFiles = fileManager.ListInputFiles(inputFolder);

            var dronesTasks = new List<Task>();
            var createDeliveryTasks = new List<Task>();
            foreach(var input in inputFiles)
            {
                //var droneId = GetDroneId(input);
                //var deliveries = await CreateDeliveries(input);
                //var drone = new Drone(deliveries, droneId);
                //dronesTasks.Add(drone.StartDelivery());
                //System.Diagnostics.Debug.WriteLine($"Drone {droneId} started...");
                
                createDeliveryTasks.Add(CreateDeliveries(input));
                System.Diagnostics.Debug.WriteLine("Deliveries created.");
            }

            while(createDeliveryTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(createDeliveryTasks);
                var deliveries = ((Task<List<Delivery>>)finishedTask).Result;
                var drone = new Drone(deliveries, deliveries[0].DroneId);
                dronesTasks.Add(drone.StartDelivery());

                System.Diagnostics.Debug.WriteLine($"Drone {deliveries[0].DroneId} started...");
                createDeliveryTasks.Remove(finishedTask);
            }

            while(dronesTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(dronesTasks);
                var output = ((Task<DeliveryOutput>)finishedTask).Result;
                await fileManager.CreateOutputFileAsync($"out{output.DroneId}.txt", output.GetContent());
                
                System.Diagnostics.Debug.WriteLine($"Drone {output.DroneId} finish.");
                dronesTasks.Remove(finishedTask);
            }
        }

        private string GetDroneId(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            if (Regex.Match(fileName, "in[0-9]+").Success)
                return fileName[2..];

            throw new ArgumentException("Input file name is wrong");
        }

        private async Task<List<Delivery>> CreateDeliveries(string inputFile)
        {
            var inputData = await fileManager.ReadDeliveryFileAsync(inputFile);
            var result = new List<Delivery>();
            foreach(var deliveryPath in inputData)
            {
                result.Add(new Delivery(deliveryPath, GetDroneId(inputFile)));
            }

            return result;
        }
    }
}
