using Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DeliveryDrone
{
    public class Delivery
    {
        public List<CharEnumerator> Routes { get; private set; }
        public string DroneId { get; }

        public Delivery(string[] routes, string droneId)
        {
            DroneId = droneId;
            SetRoutes(routes);
        }

        private void SetRoutes(string[] routes)
        {
            Routes = new List<CharEnumerator>();
            foreach (var path in routes)
            {
                if (!Regex.IsMatch(path, "^[A|I|D]+$"))
                    throw new DeliveryPathException("Delivery path should contains A, I and D characters only.", DroneId);

                Routes.Add(path.GetEnumerator());
            }
        }
    }
}
