using Infrastructure;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryDrone.IntegrationTests
{
    public class RestaurantTests
    {
        private readonly IFileManager fileManagerMock;
        private string outputFolder;

        public RestaurantTests()
        {
            fileManagerMock = new FileManager();
            outputFolder = "Output";
            Directory.CreateDirectory(outputFolder);
        }

        [Fact]
        public async Task DispatchLunchesAsync_ThreeWellFormedFilesAndOneWrongFile_NotFail()
        {
            // Arrange
            string inputFolder = "Files";
            ITransportFactory transportFactory = new TransportFactory();
            IRestaurant restaurant = new Restaurant(fileManagerMock, transportFactory);

            // Act
            await restaurant.DispatchLunchesAsync(inputFolder, TransportTypes.LightDrone);

            //Assert
            Assert.True(File.Exists($"{outputFolder}/out01.txt"));
            Assert.True(File.Exists($"{outputFolder}/out02.txt"));
            Assert.True(File.Exists($"{outputFolder}/out03.txt"));
            Assert.True(File.Exists($"{outputFolder}/out04.txt"));
        }
    }
}
