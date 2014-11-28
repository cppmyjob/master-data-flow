using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using MasterDataFlow.Contract;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;
using MasterDataFlow.Network.Packets;

namespace MasterDataFlow.Server
{
    public class WcfServer : IWcfGateContract
    {
        // TODO Create separate config section for server side configuration
        private static readonly ServerGate Gate = CreateDefaultServerGate();


        private static IWcfGateCallback _callback;

        private class GateCallback : IGateCallback
        {
            //private IWcfGateCallback Proxy
            //{
            //    get
            //    {
            //        return OperationContext.Current.GetCallbackChannel<IWcfGateCallback>();
            //    }
            //}

            public void Send(RemotePacket packet)
            {
                //Proxy.Send(packet);
                WcfServer._callback.Send(packet);
            }
        }

        // TODO implement dynamic configuration based on client request
        private static ServerGate CreateDefaultServerGate()
        {
            var result = new ServerGate(new ServiceKey(new Guid(ConfigurationManager.AppSettings["ServerGateKey"])), new GateCallback());
            var remoteCommandRunner = new CommandRunner();
            result.ConnectHub(remoteCommandRunner);
            var remoteContainer = new SimpleContainer();
            remoteCommandRunner.ConnectHub(remoteContainer);
            return result;
        }

        public void Send(RemotePacket packet)
        {
            _callback = OperationContext.Current.GetCallbackChannel<IWcfGateCallback>();
            Gate.Send(packet);
        }

    }
}
