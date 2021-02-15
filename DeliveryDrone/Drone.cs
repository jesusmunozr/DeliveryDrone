﻿using System;

namespace DeliveryDrone
{
    public class Drone
    {
        private readonly Position initialPosition => new Position { Direction = Direction.North, PosX = 0, PosY = 0 };

        public Position CurrentPosition { get; set; }

        public Position FinalPosition { get; set; }

        public short Capacity { get; set; } = 20;

        public Drone()
        {
            
        }
    }
}