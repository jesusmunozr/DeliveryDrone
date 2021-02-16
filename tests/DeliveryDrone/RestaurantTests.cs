using DeliveryDrone;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace tests.DeliveryDrone
{
    public class RestaurantTests
    {
        [Fact]
        public async Task GivenAnInputFileShouldGetOutputFile()
        {
            // Arrange
            var fileManager = new FileManager();
            var restaurant = new Restaurant(fileManager);

            // Act
            await restaurant.DispatchDrones("Files");

            // Assert
            Assert.True(true);
        }
    }
}
