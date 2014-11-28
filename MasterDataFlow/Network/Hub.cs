using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Network.Packets;
using MasterDataFlow.Network.Routing;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Network
{
    public abstract class Hub : IHub
    {
        private readonly AsyncDictionary<BaseKey, IHub> _connectedHubs = new AsyncDictionary<BaseKey, IHub>();
        private readonly AsyncQueue<IPacket> _queue = new AsyncQueue<IPacket>();
        private readonly HubAccumulator _accumulator = new HubAccumulator();

        private readonly RouteTable _routeTable = new RouteTable();
        private readonly AsyncDictionary<string, BaseKey> _routeRequests = new AsyncDictionary<string, BaseKey>(); 

        public abstract BaseKey Key { get; }

        public virtual bool ConnectHub(IHub hub)
        {
            if (hub == null) throw new ArgumentNullException("hub");
            if (hub.Key == null) throw new ArgumentNullException("hub.Key");

            // TODO What we should do if hub.Key exists? Throw an exception or silent

            _connectedHubs.AddItem(hub.Key, hub);
            // TODO handle not support cases
            hub.AcceptHub(this);
            return true;
        }

        public virtual void AcceptHub(IHub hub)
        {
            _connectedHubs.AddItem(hub.Key, hub);
        }

        public virtual void DisconnectHub(IHub hub)
        {
            throw new NotImplementedException();
        }

        public virtual SendStatus Send(IPacket packet)
        {
            _queue.Enqueue(packet);
            return SendStatus.Ok;
        }


        public IHubAccumulator Accumulator
        {
            get { return _accumulator; }
        }

        public IList<IHub> ConnectedHubs {
            get { return new ReadOnlyCollection<IHub>(_connectedHubs.GetItems()); }
        }

        protected abstract void ProccessPacket(IPacket packet);

        protected internal bool Loop()
        {
            if (_queue.Count == 0)
                return false;
            var packet = _queue.Dequeue();

            if (packet.SenderKey != Key)
            {
                var requestPacket = packet.Body as RouteRequest;
                if (requestPacket != null)
                {
                    ProcessRouteRequest(packet.SenderKey, packet.RecieverKey, requestPacket);
                    return true;
                }
                var responseRequest = packet.Body as RouteResponse;
                if (responseRequest != null)
                {
                    ProcessRouteResponse(packet.SenderKey, responseRequest);
                    return true;
                }
            }
            
            if (packet.RecieverKey == Key)
            {
                ProccessPacket(packet);
                return true;
            }

            var reciever = _connectedHubs.GetItem(packet.RecieverKey);
            if (reciever != null)
            {
                reciever.Send(packet);
            }
            else
            {
                ProcessUndeliveredPacket(packet);
            }
            return true;
        }

        protected virtual void ProcessUndeliveredPacket(IPacket packet)
        {
            if (SendPacketViaRouting(packet)) return;
            string eventKey = Guid.NewGuid().ToString();
            CreateRouteRequest(eventKey, packet.RecieverKey);
            _accumulator.Add(eventKey, packet);
        }

        protected bool SendPacketViaRouting(IPacket packet)
        {
            Route route = GetRoute(packet.RecieverKey);
            if (route != null)
            {
                var hub = _connectedHubs.GetItems().FirstOrDefault(s => s.Key == route.Gate);
                if (hub != null)
                {
                    route.LastUsage = DateTime.Now;
                    hub.Send(packet);
                    return true;
                }
            }
            return false;
        }

        protected virtual Route GetRoute(BaseKey destination)
        {
            while (true)
            {
                Route route = _routeTable.GetRoute(destination);
                if (route == null)
                    return null;
                return _connectedHubs.GetItems().FirstOrDefault(s => s.Key == route.Gate) == null ? GetRoute(route.Gate) : route;
            }
        }

        protected virtual void ProcessRouteRequest(BaseKey senderKey, BaseKey recieverKey, RouteRequest routeRequest)
        {
            if (_routeRequests.GetItem(routeRequest.RequestId) != null)
                return;

            var route = _routeTable.GetRoute(recieverKey);
            if (route != null)
            {
                CreateRouteResponse(senderKey, routeRequest);
            }

            var hub = _connectedHubs.GetItems().FirstOrDefault(s => s.Key == routeRequest.DestinationKey);
            if (hub != null)
            {
                CreateRouteResponse(senderKey, routeRequest);
                return;
            }
            _routeRequests.AddItem(routeRequest.RequestId, senderKey);
            RelayRequest(routeRequest.RequestId, routeRequest.DestinationKey);
        }

        protected virtual void ProcessRouteResponse(BaseKey senderKey, RouteResponse routeResponse)
        {
            var routeRequest = _routeRequests.GetItem(routeResponse.RequestId);
            if (routeRequest == null)
                return;

            _routeTable.AddRoutes(routeResponse.Routes);

            if (routeRequest == Key)
            {
                _accumulator.Lock(routeResponse.RequestId);
                var packet = _accumulator.Extract(routeResponse.RequestId);
                _accumulator.UnLock(routeResponse.RequestId);
                if (packet != null && packet.Count != 0)
                {
                    Send(packet.FirstOrDefault());
                    return;
                }
            }

            foreach (var route in routeResponse.Routes)
                route.IncrementLength();

            routeResponse.Routes.Add(new Route(senderKey, Key, 1));
            var nextHubKey = _routeRequests.GetItem(routeResponse.RequestId);
            if (nextHubKey == null)
                return;

            var response = new Packet(Key, nextHubKey, new RouteResponse(routeResponse.RequestId, routeResponse.Routes));
            Send(response);
        }

        private void CreateRouteRequest(string eventKey, BaseKey destinationPoint)
        {
            _routeRequests.AddItem(eventKey, Key);
            RelayRequest(eventKey, destinationPoint);
        }

        private void CreateRouteResponse(BaseKey senderKey, RouteRequest packet)
        {
            var routes = new List<Route>();
            var route = new Route(packet.DestinationKey, Key, 1);
            routes.Add(route);
            var response = new Packet(Key, senderKey, new RouteResponse(packet.RequestId, routes));
            Send(response);
        }

        protected virtual void RelayRequest(string requestId, BaseKey destinationPoint)
        {
            foreach (var hub in _connectedHubs.GetItems())
            {
                var requestPacket = new Packet(Key, hub.Key, new RouteRequest(requestId, destinationPoint));
                Send(requestPacket);
            }
        }

    }
}
