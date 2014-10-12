using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;

namespace MasterDataFlow.Network
{
    public class Gate : Hub
    {
        private readonly ServiceKey _key = new ServiceKey();

        private readonly IRemoteClientContext _context;

        public Gate(IRemoteClientContext remoteHostContract)
        {
            _context = remoteHostContract;
        }

        public override BaseKey Key
        {
            get { return _key; }
        }

        public override SendStatus Send(IPacket packet)
        {
            var result = base.Send(packet);
            EventLoop.EventLoop.QueueEvent(state => Loop());
            return result;
        }

        protected override void ProccessPacket(IPacket packet)
        {
            //var commandInfo = (CommandInfo)data;
            //var commandTypeName = commandInfo.CommandDefinition.Command.AssemblyQualifiedName;
            //var dataObject = Serializator.Serialize(commandInfo.CommandDataObject);
            //string dataObjectTypeName = commandInfo.CommandDataObject.GetType().AssemblyQualifiedName;

            ////var requestId = Guid.NewGuid();
            //// TODO Try to understand what should i do when exception is thrown during Execute
            ////_context.RegisterCallback(loopId, callback);
            //_context.Contract.Execute(commandInfo.CommandWorkflow.Key, commandInfo.CommandKey, commandTypeName, dataObjectTypeName, dataObject);

        }
    }
}
