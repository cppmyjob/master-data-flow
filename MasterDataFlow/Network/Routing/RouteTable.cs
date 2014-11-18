using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network.Routing
{
    public class RouteTable
    {
        private readonly IList<Route> _routes = new List<Route>();

        public void AddRoute(Route route)
        {
            _routes.Add(route);
        }

        public void AddRoutes(IEnumerable<Route> routes)
        {
            foreach (var route in routes)
            {
                _routes.Add(route);
            }
        }

        public Route GetRoute(BaseKey destination)
        {
            var routes = _routes.Where(s => s.Destination == destination);
            return routes.OrderBy(s => s.Lenght).FirstOrDefault();
        }

        public void DeleteRoute(BaseKey destination)
        {
            var routesForDeleting = _routes.Where(s => s.Destination == destination).ToList();
            foreach (var route in routesForDeleting)
            {
                _routes.Remove(route);
            }
        }

        public void ClearTable()
        {
            _routes.Clear();
        }

        public void CleanUpTable()
        {
            //Removing non-actual routes (for example, by LastUpdated property)
            throw new NotImplementedException();
        }
    }
}
