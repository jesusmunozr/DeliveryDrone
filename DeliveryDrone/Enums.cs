namespace DeliveryDrone
{
    public enum TransportTypes
    {
        LightDrone,
        StrongDrone,
        AutonomousVehicle
    }

    public enum CardinalPoints : short
    {
        North = 1,
        East = 2,
        South = 3,
        West = 4
    }

    public static class CardinalPintsExtensions
    {
        public static string GetName(this CardinalPoints cardinalPoint)
        {
            var name = cardinalPoint switch
            {
                CardinalPoints.North => "Norte",
                CardinalPoints.South => "Sur",
                CardinalPoints.East => "Oriente",
                CardinalPoints.West => "Occidente",
                _ => throw new System.NotImplementedException()
            };

            return name;
        }
    }
}