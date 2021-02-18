using DeliveryDrone;
using Infrastructure;
using Infrastructure.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace tests.DeliveryDrone
{
    public class RestaurantTests
    {
        private readonly Mock<IFileManager> fileManagerMock;

        public RestaurantTests()
        {
            fileManagerMock = new Mock<IFileManager>();
        }

        [Fact]
        public async Task DispatchDronesAsync_TooMuchDrones_RaiseRestaurantException()
        {
            // Arrange
            fileManagerMock.Setup(f => f.ListInputFiles("Files")).Returns(new string[21]);

            var restaurant = new Restaurant(fileManagerMock.Object);

            // Act && Assert
            var exception = await Assert.ThrowsAsync<RestaurantException>(async () => await restaurant.DispatchDronesAsync("Files"));
            Assert.Equal("Capacity of drones exceeded.", exception.Message);
        }

        [Theory]
        [InlineData("Drone capacity exceeded.", typeof(DroneException), "A", "DA", "DDA", "DDDA")]
        [InlineData("Drone out of range.", typeof(DroneException), "A", "DAAAAAAAAAAA", "DDA")]
        [InlineData("Delivery path should contains A, I and D characters only.", typeof(DeliveryPathException), "AADAAIAAR")]
        public async Task DispatchDronesAsync_FileWithDronCapacityExceeded_RaiseDronException(string expectedMessage, Type expectedExceptionType,  params string[] inputFilelines)
        {
            // Arrange
            fileManagerMock.Setup(f => f.ListInputFiles("Files")).Returns(new string[] { "Files/in01.txt" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in01.txt").Result).Returns(inputFilelines);

            var restaurant = new Restaurant(fileManagerMock.Object);

            // Act 
            await restaurant.DispatchDronesAsync("Files");

            // Assert
            Assert.True(restaurant.DeliveryStatus["01"].Fail);
            Assert.IsType(expectedExceptionType, restaurant.DeliveryStatus["01"].FailureException);
            Assert.Equal(expectedMessage, restaurant.DeliveryStatus["01"].FailureException.Message);
        }

        [Fact]
        public async Task DispatchDronesAsync_WellFormedFiles_NotFail()
        {
            // Arrange
            fileManagerMock.Setup(f => f.ListInputFiles("Files")).Returns(new string[] { "Files/in01.txt", "Files/in02.txt", "Files/in03.txt", "Files/in04.txt" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in01.txt").Result).Returns(new string[] { "AAAAIAA", "DDDAIAD", "AAIADAD" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in02.txt").Result).Returns(new string[] { "IAAAIAAAADAA", "DAAAADAAAAAAAAAAAIAAAA", "DAAAADAAAAAAAAAAAAAA" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in03.txt").Result).Returns(new string[] { "AAAAAAIAAAAAAA", "DDAAAAAAAAAAAAAAA", "DAAAAAADAAAAAAAAIAAAAA" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in04.txt").Result).Returns(new string[] { "AIAAAAADA", "DAAAAAAAAAAAAAADAA", "AADAAAAAAAAAAAAAAAA" });

            var restaurant = new Restaurant(fileManagerMock.Object);

            // Act
            await restaurant.DispatchDronesAsync("Files");

            // Assert
            Assert.False(restaurant.DeliveryStatus["01"].Fail);
            Assert.False(restaurant.DeliveryStatus["02"].Fail);
            Assert.False(restaurant.DeliveryStatus["03"].Fail);
            Assert.False(restaurant.DeliveryStatus["04"].Fail);
        }

        [Fact]
        public async Task DispatchDronesAsync_ThreeWellFormedFilesAndOneBadFile_NotFail()
        {
            // Arrange
            fileManagerMock.Setup(f => f.ListInputFiles("Files")).Returns(new string[] { "Files/in01.txt", "Files/in02.txt", "Files/in03.txt", "Files/in04.txt" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in01.txt").Result).Returns(new string[] { "AAAAIAA", "DDDAIAD", "AAIADAD" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in02.txt").Result).Returns(new string[] { "IAAAIAAAADAA", "DAAAADAAAAAXAAAAAAIAAAA", "DAAAADAAAAAAAAAAAAAA" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in03.txt").Result).Returns(new string[] { "AAAAAAIAAAAAAA", "DDAAAAAAAAAAAAAAA", "DAAAAAADAAAAAAAAIAAAAA" });
            fileManagerMock.Setup(f => f.ReadDeliveryFileAsync("Files/in04.txt").Result).Returns(new string[] { "AIAAAAADA", "DAAAAAAAAAAAAAADAA", "AADAAAAAAAAAAAAAAAA" });

            var restaurant = new Restaurant(fileManagerMock.Object);

            // Act
            await restaurant.DispatchDronesAsync("Files");

            // Assert
            Assert.False(restaurant.DeliveryStatus["01"].Fail);
            Assert.True(restaurant.DeliveryStatus["02"].Fail);
            Assert.False(restaurant.DeliveryStatus["03"].Fail);
            Assert.False(restaurant.DeliveryStatus["04"].Fail);
        }
    }
}
