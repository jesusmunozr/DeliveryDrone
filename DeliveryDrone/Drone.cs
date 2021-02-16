using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class Drone
    {
        private readonly Position initialPosition;
        private readonly List<Delivery> deliveries;
        public readonly string Id;

        public Position CurrentPosition { get; private set; }

        public Position FinalPosition { get; set; }

        public short Capacity { get; set; } = 3;

        public Drone(List<Delivery> deliveries, string droneId)
        {
            Id = droneId;
            initialPosition = new Position { Direction = Direction.North, PosX = 0, PosY = 0 };
            this.deliveries = deliveries;
            CurrentPosition = initialPosition;
        }

        public Task<DeliveryOutput> StartDelivery()
        {
            return Task.Run(() =>
            {
                var locations = new List<Position>();
                foreach (var delivery in deliveries)
                {
                    while (delivery.MoveNext())
                        Move(delivery.Current);
                    locations.Add(CurrentPosition);
                }

                var result = new DeliveryOutput(locations, Id);

                return result;
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
