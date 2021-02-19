using System;

namespace DeliveryDrone
{
    public class Location : ICloneable
    {
        public int PosX { get; private set; }
        public int PosY { get; private set; }
        public CardinalPoints Orientation { get; private set; }

        public Location()
        {
            PosX = 0;
            PosY = 0;
            Orientation = CardinalPoints.North;
        }

        private Location(int x, int y, CardinalPoints c)
        {
            PosX = x;
            PosY = y;
            Orientation = c;
        }

        public void NextStep(char step)
        {
            switch (step)
            {
                case 'A':
                    if (Orientation == CardinalPoints.North)
                        PosY += 1;
                    else if (Orientation == CardinalPoints.South)
                        PosY -= 1;
                    else if (Orientation == CardinalPoints.East)
                        PosX += 1;
                    else // West
                        PosX -= 1;
                    break;
                case 'I':
                    Orientation = TurnLeft();
                    break;
                case 'D':
                    Orientation = TurnRight();
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public bool IsValid(short maximumDistance)
        {
            return (PosX <= maximumDistance && PosX >= -maximumDistance && PosY <= maximumDistance && PosY >= -maximumDistance);
        }

        public object Clone()
        {
            return new Location(PosX, PosY, Orientation);
        }

        private CardinalPoints TurnLeft()
        {
            short currentValue = (short)Orientation;
            currentValue--;

            if (currentValue == 0)
                currentValue = 4;

            return (CardinalPoints)currentValue;
        }

        private CardinalPoints TurnRight()
        {
            short currentValue = (short)Orientation;
            currentValue++;

            if (currentValue == 5)
                currentValue = 1;

            return (CardinalPoints)currentValue;
        }


    }
}