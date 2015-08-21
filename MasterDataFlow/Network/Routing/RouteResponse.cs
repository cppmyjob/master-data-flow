using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network.Routing
{
    [Serializable]
    public class RouteResponse
    {
        public RouteResponse()
        {
            
        }

        public RouteResponse(string requestId, List<Route> routes)
        {
            RequestId = requestId;
            Routes = routes;
        }

        public string RequestId { get; set; }

        public List<Route> Routes { get; set; }
    }
}
