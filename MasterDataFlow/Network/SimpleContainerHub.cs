using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network
{
    public class SimpleContainerHub : EventLoopHub
    {
        private ServiceKey _key = new ServiceKey();
        private CommandRunnerHub _runner;

        public override BaseKey Key
        {
            get { return _key; }
        }

        public override void AcceptHub(IHub hub)
        {
            _runner = (CommandRunnerHub) hub;
        }

        protected override void ProccessPacket(IPacket packet)
        {
            var action = packet.Body as LocalExecuteCommandAction;
            if (action == null)
                // TODO Send Error packet
                return;

            try
            {
                var commandInfo = action.CommandInfo;
                var commandInstance = commandInfo.CommandDefinition.CreateInstance(commandInfo.CommandKey, commandInfo.CommandDataObject);
                var result = commandInstance.BaseExecute();
                var resultPacket = new Packet(Key, commandInfo.WorkflowKey, result);
                _runner.Send(resultPacket);
            }
            catch (Exception ex)
            {
                // TODO Send Error Packet
            }
            finally
            {
                //_commandInstance = null;
            }

        }
    }
}
