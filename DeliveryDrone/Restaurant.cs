using Infrastructure;
using Infrastructure.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class Restaurant : IRestaurant
    {
        private const short CAPACITY_OF_DRONES = 20;
        private readonly IFileManager fileManager;

        public IDictionary<string, DeliveryOutput> DeliveryStatus { get; set; } = new Dictionary<string, DeliveryOutput>();

        public Restaurant(IFileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        public async Task DispatchDronesAsync(string inputFolder)
        {
            try
            {
                var inputFiles = fileManager.ListInputFiles(inputFolder);

                if (inputFiles.Length > CAPACITY_OF_DRONES)
                    throw new RestaurantException("Capacity of drones exceeded.");

                var createDeliveryTasks = new List<Task>();

                foreach (var input in inputFiles)
                {
                    createDeliveryTasks.Add(CreateDeliveries(input));
                    System.Diagnostics.Debug.WriteLine("Deliveries created.");
                }

                var dronesTasks = StartDeliveriesAsync(createDeliveryTasks);
                //var dronesTasks = new List<Task>();

                //while (createDeliveryTasks.Count > 0)
                //{
                //    Task finishedTask = await Task.WhenAny(createDeliveryTasks);

                //    if(finishedTask.Exception is not null)
                //    {
                //        var concreteException = (DeliveryPathException)finishedTask.Exception.InnerException;
                //        var output = new DeliveryOutput(concreteException.DroneId, concreteException, fileManager);
                //        DeliveryStatus.Add(output.DroneId, output);

                //        // Save file with the output information
                //        await output.SaveAsync();

                //        createDeliveryTasks.Remove(finishedTask);
                //        continue;
                //    }

                //    var deliveries = ((Task<List<Delivery>>)finishedTask).Result;
                //    var drone = new Drone(deliveries, deliveries[0].DroneId);
                //    dronesTasks.Add(drone.StartDelivery(fileManager));

                //    System.Diagnostics.Debug.WriteLine($"Drone {deliveries[0].DroneId} started...");
                //    createDeliveryTasks.Remove(finishedTask);
                //}

                await SaveResultsAsync(dronesTasks);
                //while (dronesTasks.Count > 0)
                //{
                //    Task finishedTask = await Task.WhenAny(dronesTasks);

                //    if (finishedTask.Exception is not null)
                //        throw finishedTask.Exception;

                //    var output = ((Task<DeliveryOutput>)finishedTask).Result;
                //    await output.SaveAsync();

                //    System.Diagnostics.Debug.WriteLine($"Drone {output.DroneId} finish.");
                //    dronesTasks.Remove(finishedTask);
                //}
            }
            catch
            {
                throw;
            }
        }
        
        private async Task<List<Task>> StartDeliveriesAsync(List<Task> createDeliveryTasks)
        {
            var dronesTasks = new List<Task>();
            while (createDeliveryTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(createDeliveryTasks);

                if (finishedTask.Exception is not null)
                {
                    var concreteException = (DeliveryPathException)finishedTask.Exception.InnerException;
                    var output = new DeliveryOutput(concreteException.DroneId, concreteException, fileManager);
                    DeliveryStatus.Add(output.DroneId, output);

                    // Save file with the output information
                    await output.SaveAsync();

                    createDeliveryTasks.Remove(finishedTask);
                    continue;
                }

                var deliveries = ((Task<List<Delivery>>)finishedTask).Result;
                var drone = new Drone(deliveries, deliveries[0].DroneId);
                dronesTasks.Add(drone.StartDelivery(fileManager));

                System.Diagnostics.Debug.WriteLine($"Drone {deliveries[0].DroneId} started...");
                createDeliveryTasks.Remove(finishedTask);
            }
            return dronesTasks;
        }

        private async Task SaveResultsAsync(Task<List<Task>> startDeliveryTask)
        {
            if (startDeliveryTask.Exception is not null)
            {
                var concreteException = (DroneException)startDeliveryTask.Exception.InnerException;
                var deliveryOutputFailed = new DeliveryOutput(concreteException.DroneId, concreteException, fileManager);
                DeliveryStatus.Add(deliveryOutputFailed.DroneId, deliveryOutputFailed);

                // Save file with the output information
                await deliveryOutputFailed.SaveAsync();
                return;
            }

            var dronesTasks = await startDeliveryTask;

            while (dronesTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(dronesTasks);

                if (finishedTask.Exception is not null)
                {
                    var concreteException = (DroneException)finishedTask.Exception.InnerException;
                    var deliveryOutputFailed = new DeliveryOutput(concreteException.DroneId, concreteException, fileManager);
                    DeliveryStatus.Add(deliveryOutputFailed.DroneId, deliveryOutputFailed);

                    // Save file with the output information
                    await deliveryOutputFailed.SaveAsync();

                    dronesTasks.Remove(finishedTask);
                    continue;
                }

                var output = ((Task<DeliveryOutput>)finishedTask).Result;
                await output.SaveAsync();

                DeliveryStatus.Add(output.DroneId, output);
                System.Diagnostics.Debug.WriteLine($"Drone {output.DroneId} finish.");
                dronesTasks.Remove(finishedTask);
            }
        }

        private string GetDroneId(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            return fileName[2..];
        }

        private async Task<List<Delivery>> CreateDeliveries(string inputFile)
        {
            var inputData = await fileManager.ReadDeliveryFileAsync(inputFile);
            var result = new List<Delivery>();
            var droneId = GetDroneId(inputFile);
            foreach (var deliveryPath in inputData)
            {
                if (!Regex.IsMatch(deliveryPath, "^[A|I|D]+$"))
                    throw new DeliveryPathException("Delivery path should contains A, I and D characters only.", droneId);

                result.Add(new Delivery(deliveryPath, droneId));
            }

            return result;
        }
    }
}
