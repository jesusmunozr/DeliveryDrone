using DeliveryDrone;
using Infrastructure;
using Infrastructure.Exceptions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace tests.DeliveryDrone
{
    public class DroneTests
    {
        private readonly Mock<IFileManager> fileManagerMock;

        public DroneTests()
        {
            fileManagerMock = new Mock<IFileManager>();
        }

        [Fact]
        public async Task StartDelivery_MoreThanThreeDeliveries_RaiseException()
        {
            // Arrange
            var droneId = "01";
            var mock = new Mock<IList<Delivery>>();
            mock.Setup(l => l.Count).Returns(4);

            var drone = new Drone(mock.Object, droneId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DroneException>(async () => await drone.StartDelivery(fileManagerMock.Object));
            Assert.Equal("Drone capacity exceeded.", exception.Message);
        }

        [Fact]
        public async Task StartDelivery_GivenValidDeliveryPath_ReturnValidDeliveryOutput()
        {
            IList<Delivery> list = new List<Delivery>();

            // Arrange
            var droneId = "01";
            var path = "AAIADAD";

            list.Add(new Delivery(path, droneId));

            var deliveryList = new Mock<IList<Delivery>>();
            deliveryList.Setup(l => l.Count).Returns(1);
            deliveryList.Setup(l => l.GetEnumerator()).Returns(list.GetEnumerator());

            var drone = new Drone(deliveryList.Object, droneId);

            // Act
            var deliveryOutput = await drone.StartDelivery(fileManagerMock.Object);

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
        public async Task StartDelivery_PathsExceedLimits_RaiseDroneException(string path)
        {
            IList<Delivery> list = new List<Delivery>();

            // Arrange
            var droneId = "01";

            list.Add(new Delivery(path, droneId));

            var deliveryList = new Mock<IList<Delivery>>();
            deliveryList.Setup(l => l.Count).Returns(1);
            deliveryList.Setup(l => l.GetEnumerator()).Returns(list.GetEnumerator());

            var drone = new Drone(deliveryList.Object, droneId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DroneException>(async () => await drone.StartDelivery(fileManagerMock.Object));
            Assert.Equal("Drone out of range.", exception.Message);
        }
    }
}
