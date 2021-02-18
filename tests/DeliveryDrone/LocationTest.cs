using DeliveryDrone;
using Xunit;

namespace tests.DeliveryDrone
{
    public class LocationTest
    {
        [Theory]
        [InlineData("A", 0, 1, CardinalPoints.North)]
        [InlineData("DDA", 0, -1, CardinalPoints.South)]
        [InlineData("DA", 1, 0, CardinalPoints.East)]
        [InlineData("IA", -1, 0, CardinalPoints.West)]
        [InlineData("I", 0, 0, CardinalPoints.West)]
        [InlineData("DDDD", 0, 0, CardinalPoints.North)]
        public void MoveNext_StepWithDifferentOrientations_ShouldUpdateLocation(string steps, int expectedX, int expectedY, CardinalPoints expectedOrientation)
        {
            // Arrange
            var currentLocation = new Location();

            // Act
            foreach (var step in steps)
                currentLocation.NextStep(step);

            //Assert
            Assert.Equal(expectedX, currentLocation.PosX);
            Assert.Equal(expectedY, currentLocation.PosY);
            Assert.Equal(expectedOrientation, currentLocation.Orientation);
        }
    }
}
