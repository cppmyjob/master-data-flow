using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network.Routing
{
    public class RouteRequest
    {

        public RouteRequest()
        {
            
        }

        public RouteRequest(string requestId, BaseKey destinationKey)
        {
            RequestId = requestId;
            DestinationKey = destinationKey;
        }

        public string RequestId { get; set; }

        public BaseKey DestinationKey { get; set; }

    }
}
