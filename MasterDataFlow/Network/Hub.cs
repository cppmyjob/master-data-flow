using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
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
        private readonly Dictionary<string, BaseKey> _routeRequests = new Dictionary<string, BaseKey>(); 

        public abstract BaseKey Key { get; }

        public virtual bool ConnectHub(IHub hub)
        {
            // TODO check if exists
            _connectedHubs.AddItem(hub.Key, hub);
            // TODO handle not support cases
            //hub.AcceptHub(this);
            return true;
        }

        public virtual void AcceptHub(IHub hub)
        {
            throw new NotSupportedException();
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

        protected abstract void ProccessPacket(IPacket packet);

        protected internal bool Loop()
        {
            if (_queue.Count == 0)
                return false;
            var packet = _queue.Dequeue();

            if (packet is RequestPacket && ((RequestPacket) packet).PacketType == PacketType.RouteRequest &&
                packet.SenderKey != Key)
            {
                ProcessRouteRequest(packet as RequestPacket);
                return true;
            }

            if (packet is RequestPacket && ((RequestPacket) packet).PacketType == PacketType.RouteResponse &&
                packet.SenderKey != Key)
            {
                ProcessRouteResponse(packet as RequestPacket);
                return true;
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
            Route route = GetRoute(packet.RecieverKey);
            if (route != null)
            {
                var hub = _connectedHubs.GetItems().FirstOrDefault(s => s.Key == route.Gate);
                if (hub != null)
                {
                    route.LastUsage = DateTime.Now;
                    hub.Send(packet);
                    return;
                }
            }
            string eventKey = Guid.NewGuid().ToString();
            CreateRouteRequest(eventKey, packet.RecieverKey);
            _accumulator.Add(eventKey, packet);
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

        protected virtual void ProcessRouteRequest(RequestPacket routeRequest)
        {
            if (_routeRequests.ContainsKey(routeRequest.RequestId))
                return;

            var route = _routeTable.GetRoute(routeRequest.RecieverKey);
            if (route != null)
            {
                CreateRouteResponse(routeRequest);
            }

            var destinationPoint = routeRequest.Body as BaseKey;
            if (destinationPoint != null)
            {
                var hub = _connectedHubs.GetItems().FirstOrDefault(s => s.Key == destinationPoint);
                if (hub != null)
                {
                    CreateRouteResponse(routeRequest);
                    return;
                }
                _routeRequests.Add(routeRequest.RequestId, routeRequest.SenderKey);
                RelayRequest(routeRequest.RequestId, destinationPoint);
            }
        }

        protected virtual void ProcessRouteResponse(RequestPacket routeResponse)
        {
            var routeRequest = _routeRequests.FirstOrDefault(s => s.Key == routeResponse.RequestId);
            if (routeRequest.Value == null)
                return;

            var routeList = routeResponse.Body as List<Route>;
            if (routeList == null)
                return;

            _routeTable.AddRoutes(routeList);

            if (routeRequest.Value == Key)
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

            foreach (var route in routeList)
                route.IncrementLenght();

            routeList.Add(new Route(routeResponse.SenderKey, Key, 1));
            BaseKey nextHub;
            if (!_routeRequests.TryGetValue(routeResponse.RequestId, out nextHub))
                return;

            var response = new RequestPacket(Key, nextHub, routeList, routeResponse.RequestId, PacketType.RouteResponse);
            Send(response);
        }

        private void CreateRouteRequest(string eventKey, BaseKey destinationPoint)
        {
            _routeRequests.Add(eventKey, Key);
            RelayRequest(eventKey, destinationPoint);
        }

        private void CreateRouteResponse(RequestPacket packet)
        {
            var destinationPoint = packet.Body as BaseKey;
            if (destinationPoint == null)
                return;
            List<Route> routes = new List<Route>();
            Route route = new Route(destinationPoint, Key, 1);
            routes.Add(route);
            var response = new RequestPacket(Key, packet.SenderKey, routes, packet.RequestId, PacketType.RouteResponse);
            Send(response);
        }

        private void RelayRequest(string requestId, BaseKey destinationPoint)
        {
            foreach (var hub in _connectedHubs.GetItems())
            {
                var requestPacket = new RequestPacket(Key, hub.Key, destinationPoint, requestId, PacketType.RouteRequest);
                Send(requestPacket);
            }
        }

    }
}
