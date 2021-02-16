using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryDrone
{
    public class Delivery : IEnumerator<char>
    {
        private readonly char[] steps;
        private int position = -1;

        public Delivery(string path)
        {
            steps = path.ToCharArray();
        }

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

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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
