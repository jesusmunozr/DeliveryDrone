using Infrastructure;
using Infrastructure.Exceptions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryDrone.Tests
{
    public class DroneTests
    {
        private readonly Mock<IFileManager> fileManagerMock;

        public DroneTests()
        {
            fileManagerMock = new Mock<IFileManager>();
        }

        [Fact]
        public async Task DeliverAsync_MoreThanThreeDeliveries_RaiseException()
        {
            // Arrange
            var droneId = "01";
            string[] paths = { "AAIADAD", "AAIADAD", "AAIADAD", "AAIADAD" };
            var deliveryMock = new Mock<Delivery>(paths, droneId);

            var drone = new Drone();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TransportException>(async () => await drone.DeliverAsync(fileManagerMock.Object, deliveryMock.Object));
            Assert.Equal("Drone capacity exceeded.", exception.Message);
        }

        [Fact]
        public async Task DeliverAsync_GivenValidDeliveryPath_ReturnValidDeliveryOutput()
        {
            IList<Delivery> list = new List<Delivery>();

            // Arrange
            var droneId = "01";
            string[] paths = { "AAIADAD" };
            var deliveryMock = new Mock<Delivery>(paths, droneId);
            var drone = new Drone();

            // Act
            var deliveryOutput = await drone.DeliverAsync(fileManagerMock.Object, deliveryMock.Object);

            // Assert
            Assert.NotNull(deliveryOutput);
            Assert.Collection<Location>(deliveryOutput.DeliveryLocations,
                item =>
                {
                    Assert.Equal(-1, item.PosX);
                    Assert.Equal(3, item.PosY);
                    Assert.Equal(CardinalPoints.East, item.Orientation);
                });
        }

        [Theory]
        [InlineData("AAAAAAAAAAA")]
        [InlineData("DAAAAAAAAAAA")]
        [InlineData("DDAAAAAAAAAAA")]
        [InlineData("DDDAAAAAAAAAAA")]
        public async Task DeliverAsync_PathsExceedLimits_RaiseTransportException(string path)
        {
            IList<Delivery> list = new List<Delivery>();

            // Arrange
            var droneId = "01";
            string[] paths = { path };

            var deliveryMock = new Mock<Delivery>(paths, droneId);

            var drone = new Drone();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TransportException>(async () => await drone.DeliverAsync(fileManagerMock.Object, deliveryMock.Object));
            Assert.Equal("Drone out of range.", exception.Message);
        }
    }
}
