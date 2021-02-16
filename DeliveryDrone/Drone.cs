using System;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class Drone
    {
        private readonly Position initialPosition;
        private readonly Delivery[] deliveries;

        public event EventHandler<Position> OnDeliveryFinished;

        public Position CurrentPosition { get; set; }

        public Position FinalPosition { get; set; }

        public short Capacity { get; set; } = 3;

        public Drone(string[] paths)
        {
            initialPosition = new Position { Direction = Direction.North, PosX = 0, PosY = 0 };

            deliveries = new Delivery[3];
            for(var i = 0; i<paths.Length; i++)
            {
                deliveries[i] = new Delivery(paths[0]);
            }
        }

        public Task Fly()
        {
            return Task.Run(() =>
            {
                foreach (var delivery in deliveries)
                {
                    while (delivery.MoveNext())
                    {
                        Move(delivery.Current);
                    }

                    // Notify the lunch has been delivered
                    if (OnDeliveryFinished != null)
                        OnDeliveryFinished(this, CurrentPosition);
                }
            });
        }

        private void Move(char step)
        {
            switch (step)
            {
                case 'A':
                    if (CurrentPosition.Direction == Direction.North)
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX, PosY = CurrentPosition.PosY + 1, Direction = Direction.North };
                    }
                    else if (CurrentPosition.Direction == Direction.South)
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX, PosY = CurrentPosition.PosY - 1, Direction = Direction.South };
                    }
                    else if (CurrentPosition.Direction == Direction.East)
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX + 1, PosY = CurrentPosition.PosY, Direction = Direction.East };
                    } 
                    else // West
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX - 1, PosY = CurrentPosition.PosY, Direction = Direction.West };
                    }
                    break;
                case 'I':
                    if(CurrentPosition.Direction == Direction.North)
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX, PosY = CurrentPosition.PosY, Direction = Direction.West };
                    }
                    else if (CurrentPosition.Direction == Direction.South)
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX, PosY = CurrentPosition.PosY, Direction = Direction.East };
                    }
                    else if (CurrentPosition.Direction == Direction.East)
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX, PosY = CurrentPosition.PosY, Direction = Direction.North };
                    }
                    else // West
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX, PosY = CurrentPosition.PosY, Direction = Direction.South };
                    }
                    break;
                case 'D':
                    if (CurrentPosition.Direction == Direction.North)
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX, PosY = CurrentPosition.PosY, Direction = Direction.East };
                    }
                    else if (CurrentPosition.Direction == Direction.South)
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX, PosY = CurrentPosition.PosY, Direction = Direction.West };
                    }
                    else if (CurrentPosition.Direction == Direction.East)
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX, PosY = CurrentPosition.PosY, Direction = Direction.South };
                    }
                    else // West
                    {
                        CurrentPosition = new Position { PosX = CurrentPosition.PosX, PosY = CurrentPosition.PosY, Direction = Direction.North };
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
