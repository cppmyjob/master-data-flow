using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MasterDataFlow.Contract;
using MasterDataFlow.Keys;
using MasterDataFlow.Network;

namespace MasterDataFlow.Server
{
    public class WcfServer : IWcfGateContract
    {
        // TODO Create separate config section for server side configuration
        private static readonly ServerGate Gate = CreateDefaultServerGate();

        // TODO implement dynamic configuration based on client request
        private static ServerGate CreateDefaultServerGate()
        {
            var result = new ServerGate(new ServiceKey(new Guid(ConfigurationManager.AppSettings["ServerGateKey"])));
            var remoteCommandRunner = new CommandRunnerHub();
            result.ConnectHub(remoteCommandRunner);
            var remoteContainer = new SimpleContainerHub();
            remoteCommandRunner.ConnectHub(remoteContainer);
            return result;
        }

        public void UploadAssembly(byte[] data)
        {
            Gate.UploadAssembly(data);
        }

        public void Send(RemotePacket packet)
        {
            Gate.Send(packet);
        }
    }
}
