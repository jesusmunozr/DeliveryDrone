using Infrastructure;
using Infrastructure.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class Restaurant : IRestaurant
    {
        private const short CAPACITY_OF_DRONES = 20;
        private readonly IFileManager fileManager;
        private readonly ITransportFactory transportFactory;

        public IDictionary<string, DeliveryOutput> DeliveryStatus { get; set; } = new Dictionary<string, DeliveryOutput>();

        public Restaurant(IFileManager fileManager, ITransportFactory factory)
        {
            this.fileManager = fileManager;
            transportFactory = factory;
        }

        public async Task DispatchLunchesAsync(string inputFolder, TransportTypes transportType)
        {
            try
            {
                var inputFiles = fileManager.ListInputFiles(inputFolder);

                if (inputFiles.Length > CAPACITY_OF_DRONES)
                    throw new RestaurantException("Capacity of drones exceeded.");

                var createDeliveryTasks = new List<Task>();

                // Create input files
                foreach (var input in inputFiles)
                    createDeliveryTasks.Add(CreateDeliveries(input));

                // Start deliveries and create the drones
                var dronesTasks = StartDeliveriesAsync(createDeliveryTasks, transportType);

                // Save the delivery result
                await SaveResultsAsync(dronesTasks);
            }
            catch
            {
                throw;
            }
        }

        private async Task<List<Task>> StartDeliveriesAsync(List<Task> createDeliveryTasks, TransportTypes transportType)
        {
            var dronesTasks = new List<Task>();
            while (createDeliveryTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(createDeliveryTasks);

                if (finishedTask.Exception is not null)
                    await ManageFailedTaskAsync(finishedTask);
                else
                {
                    var delivery = ((Task<Delivery>)finishedTask).Result;
                    var transport = transportFactory.CreateTransport(transportType);
                    dronesTasks.Add(transport.DeliverAsync(fileManager, delivery));
                }

                createDeliveryTasks.Remove(finishedTask);
            }
            return dronesTasks;
        }

        private async Task SaveResultsAsync(Task<List<Task>> startDeliveryTask)
        {
            if (startDeliveryTask.Exception is not null)
            {
                await ManageFailedTaskAsync(startDeliveryTask);
                return;
            }

            var dronesTasks = await startDeliveryTask;

            while (dronesTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(dronesTasks);

                if (finishedTask.Exception is not null)
                    await ManageFailedTaskAsync(finishedTask);
                else
                {
                    var output = ((Task<DeliveryOutput>)finishedTask).Result;
                    await output.SaveAsync();
                    DeliveryStatus.Add(output.DroneId, output);
                }
                dronesTasks.Remove(finishedTask);
            }
        }

        private async Task ManageFailedTaskAsync(Task finishedTask)
        {
            var concreteException = (ExceptionBase)finishedTask.Exception.InnerException;
            var deliveryOutput = new DeliveryOutput(concreteException, fileManager);
            DeliveryStatus.Add(deliveryOutput.DroneId, deliveryOutput);

            // Save file with output information
            await deliveryOutput.SaveAsync();
        }

        private string GetDroneId(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            return fileName[2..];
        }

        private async Task<Delivery> CreateDeliveries(string inputFile)
        {
            var inputData = await fileManager.ReadDeliveryFileAsync(inputFile);
            var result = new Delivery(inputData, GetDroneId(inputFile));
            return result;
        }
    }
}
