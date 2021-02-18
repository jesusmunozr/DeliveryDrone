using System;
using System.Collections;
using System.Collections.Generic;

namespace DeliveryDrone
{
    public class Delivery : IEnumerable<char>
    {
        private char[] steps;
        public string DroneId { get; }

        public Delivery(string path, string droneId)
        {
            steps = path.ToCharArray();
            DroneId = droneId;
        }

        public IEnumerator<char> GetEnumerator()
        {
            return new DeliveryEnum(steps);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class DeliveryEnum : IEnumerator<char>
    {
        private char[] steps;
        private int position = -1;

        public char Current
        {
            get
            {
                try
                {
                    return steps[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current => Current;

        public DeliveryEnum(char[] list)
        {
            steps = list;
        }

        public void Dispose()
        {
            Reset();
            steps = null;
        }

        public bool MoveNext()
        {
            position++;
            return (position < steps.Length);
        }

        public void Reset()
        {
            position = -1;
        }
    }
}
