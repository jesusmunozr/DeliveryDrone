using Infrastructure;
using Infrastructure.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryDrone.Tests
{
    public class RestaurantTests
    {
        private readonly Mock<IFileManager> fileManagerMock;
        private readonly Mock<ITransportFactory> transportFactoryMock;

        public RestaurantTests()
        {
            fileManagerMock = new Mock<IFileManager>();
            transportFactoryMock = new Mock<ITransportFactory>();
        }

        [Fact]
        public async Task DispatchLunchesAsync_TooMuchDrones_RaiseRestaurantException()
        {
            // Arrange
            fileManagerMock.Setup(f => f.ListInputFiles("Files")).Returns(new string[21]);
            transportFactoryMock.Setup(t => t.CreateTransport(TransportTypes.LightDrone));

            var restaurant = new Restaurant(fileManagerMock.Object, transportFactoryMock.Object);

            // Act && Assert
            var exception = await Assert.ThrowsAsync<RestaurantException>(async () => await restaurant.DispatchLunchesAsync("Files", TransportTypes.LightDrone));
            Assert.Equal("Capacity of drones exceeded.", exception.Message);
        }

        [Theory]
        [InlineData("Drone capacity exceeded.", typeof(TransportException), "A", "DA", "DDA", "DDDA")]
        [InlineData("Drone out of range.", typeof(TransportException), "A", "DAAAAAAAAAAA", "DDA")]
        [InlineData("Delivery path should contains A, I and D characters only.", typeof(DeliveryPathException), "AADAAIAAR")]
        public async Task DispatchLunchesAsync_FileWithDronCapacityExceeded_RaiseDronException(string expectedMessage, Type expectedExceptionType, params string[] inputFilelines)
        {
            // Arrange
            fileManagerMock.Setup(f => f.ListInputFiles("Files")).Returns(new string[] { "Files/in01.txt" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in01.txt").Result).Returns(inputFilelines);
            transportFactoryMock.Setup(t => t.CreateTransport(TransportTypes.LightDrone)).Returns(new Drone());

            var restaurant = new Restaurant(fileManagerMock.Object, transportFactoryMock.Object);

            // Act 
            await restaurant.DispatchLunchesAsync("Files", TransportTypes.LightDrone);

            // Assert
            Assert.True(restaurant.DeliveryStatus["01"].Fail);
            Assert.IsType(expectedExceptionType, restaurant.DeliveryStatus["01"].FailureException);
            Assert.Equal(expectedMessage, restaurant.DeliveryStatus["01"].FailureException.Message);
        }

        [Fact]
        public async Task DispatchLunchesAsync_WellFormedFiles_NotFail()
        {
            // Arrange
            fileManagerMock.Setup(f => f.ListInputFiles("Files")).Returns(new string[] { "Files/in01.txt", "Files/in02.txt", "Files/in03.txt", "Files/in04.txt" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in01.txt").Result).Returns(new string[] { "AAAAIAA", "DDDAIAD", "AAIADAD" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in02.txt").Result).Returns(new string[] { "IAAAIAAAADAA", "DAAAADAAAAAAAAAAAIAAAA", "DAAAADAAAAAAAAAAAAAA" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in03.txt").Result).Returns(new string[] { "AAAAAAIAAAAAAA", "DDAAAAAAAAAAAAAAA", "DAAAAAADAAAAAAAAIAAAAA" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in04.txt").Result).Returns(new string[] { "AIAAAAADA", "DAAAAAAAAAAAAAADAA", "AADAAAAAAAAAAAAAAAA" });

            var deliveryOutputMock1 = new Mock<DeliveryOutput>(new List<Location>(), "01", fileManagerMock.Object);
            var deliveryOutputMock2 = new Mock<DeliveryOutput>(new List<Location>(), "02", fileManagerMock.Object);
            var deliveryOutputMock3 = new Mock<DeliveryOutput>(new List<Location>(), "03", fileManagerMock.Object);
            var deliveryOutputMock4 = new Mock<DeliveryOutput>(new List<Location>(), "04", fileManagerMock.Object);

            var droneMock = new Mock<ITransport>();
            droneMock.SetupSequence(d => d.DeliverAsync(fileManagerMock.Object, It.IsAny<Delivery>()).Result)
                .Returns(deliveryOutputMock1.Object)
                .Returns(deliveryOutputMock2.Object)
                .Returns(deliveryOutputMock3.Object)
                .Returns(deliveryOutputMock4.Object);
            transportFactoryMock.Setup(t => t.CreateTransport(TransportTypes.LightDrone)).Returns(droneMock.Object);

            var restaurant = new Restaurant(fileManagerMock.Object, transportFactoryMock.Object);

            // Act
            await restaurant.DispatchLunchesAsync("Files", TransportTypes.LightDrone);

            // Assert
            Assert.False(restaurant.DeliveryStatus["01"].Fail);
            Assert.False(restaurant.DeliveryStatus["02"].Fail);
            Assert.False(restaurant.DeliveryStatus["03"].Fail);
            Assert.False(restaurant.DeliveryStatus["04"].Fail);
        }

        [Fact]
        public async Task DispatchLunchesAsync_ThreeWellFormedFilesAndOneBadFile_NotFail()
        {
            // Arrange
            fileManagerMock.Setup(f => f.ListInputFiles("Files")).Returns(new string[] { "Files/in01.txt", "Files/in02.txt", "Files/in03.txt", "Files/in04.txt" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in01.txt").Result).Returns(new string[] { "AAAAIAA", "DDDAIAD", "AAIADAD" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in02.txt").Result).Returns(new string[] { "IAAAIAAAADAA", "DAAAADAAAAAXAAAAAAIAAAA", "DAAAADAAAAAAAAAAAAAA" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in03.txt").Result).Returns(new string[] { "AAAAAAIAAAAAAA", "DDAAAAAAAAAAAAAAA", "DAAAAAADAAAAAAAAIAAAAA" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in04.txt").Result).Returns(new string[] { "AIAAAAADA", "DAAAAAAAAAAAAAADAA", "AADAAAAAAAAAAAAAAAA" });

            var deliveryOutputMock1 = new Mock<DeliveryOutput>(new List<Location>(), "01", fileManagerMock.Object);
            var deliveryOutputMock3 = new Mock<DeliveryOutput>(new List<Location>(), "03", fileManagerMock.Object);
            var deliveryOutputMock4 = new Mock<DeliveryOutput>(new List<Location>(), "04", fileManagerMock.Object);

            var droneMock = new Mock<ITransport>();
            droneMock.SetupSequence(d => d.DeliverAsync(fileManagerMock.Object, It.IsAny<Delivery>()).Result)
                .Returns(deliveryOutputMock1.Object)
                .Returns(deliveryOutputMock3.Object)
                .Returns(deliveryOutputMock4.Object);
            transportFactoryMock.Setup(t => t.CreateTransport(TransportTypes.LightDrone)).Returns(droneMock.Object);

            var restaurant = new Restaurant(fileManagerMock.Object, transportFactoryMock.Object);

            // Act
            await restaurant.DispatchLunchesAsync("Files", TransportTypes.LightDrone);

            // Assert
            Assert.False(restaurant.DeliveryStatus["01"].Fail);
            Assert.True(restaurant.DeliveryStatus["02"].Fail);
            Assert.False(restaurant.DeliveryStatus["03"].Fail);
            Assert.False(restaurant.DeliveryStatus["04"].Fail);
        }
    }
}
