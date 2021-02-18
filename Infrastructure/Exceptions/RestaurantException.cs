using System;

namespace Infrastructure.Exceptions
{
    public class RestaurantException : Exception
    {
        public RestaurantException(string message)
            : base(message)
        {

        }
    }
}
