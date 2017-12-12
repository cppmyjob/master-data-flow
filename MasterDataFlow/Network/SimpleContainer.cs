using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Actions;
using MasterDataFlow.Assemblies;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network.Packets;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Network
{
    public class SimpleContainer : EventLoopHub, IMessageSender
    {
        private readonly ServiceKey _key = new ServiceKey();
        private CommandRunner _runner;

        public override BaseKey Key
        {
            get { return _key; }
        }

        public override void AcceptHub(IHub hub)
        {
            // TODO Check CommandRunnerHub
            _runner = (CommandRunner) hub;
        }

        protected override void ProccessPacket(IPacket packet)
        {
            var action = packet.Body as LocalExecuteCommandAction;
            if (action == null)
                // TODO Send Error packet
                return;

            if (action.CommandInfo != null)
                ExecuteLocalDomain(action.CommandInfo);
            else
                ExecuteExternalDomain(action.ExternalDomainCommandInfo);
        }

        private void ExecuteExternalDomain(ExternalDomainCommandInfo commandInfo)
        {
            try
            {
                Logger.Instance.Debug("Starting command : {0}", commandInfo.CommandType);
                Type resultType;
                var result = commandInfo.AssemblyLoader.LocalExecuteCommandAction(
                    commandInfo.WorkflowKey,
                    commandInfo.CommandType,
                    commandInfo.DataObject,
                    commandInfo.DataObjectType,
                    commandInfo.CommandKey,
                    out resultType, this);
                Logger.Instance.Debug("Finished command : {0}", commandInfo.CommandType);
                var message = new SerializedCommandMessage((CommandKey) BaseKey.DeserializeKey(commandInfo.CommandKey));
                message.Data = result;
                message.DataType = resultType;
                var resultPacket = new Packet(Key, commandInfo.WorkflowKey, message);
                Logger.Instance.Debug("Sending command result to {0}", commandInfo.WorkflowKey);
                _runner.Send(resultPacket);
            }
            catch (Exception ex)
            {
                // TODO Send Error Packet
                Logger.Instance.Error("SimpleContainer::ExecuteExternalDomain", ex);
            }
        }

        private void ExecuteLocalDomain(LocalDomainCommandInfo commandInfo)
        {
            try
            {
                Logger.Instance.Debug("Starting command execution");
                var commandInstance = Creator.CreateCommandInstance(commandInfo.WorkflowKey, commandInfo.CommandKey, 
                    commandInfo.CommandType, commandInfo.CommandDataObject, this);
                var result = commandInstance.BaseExecute();
                Logger.Instance.Debug("Finished command execution");
                var resultPacket = new Packet(Key, commandInfo.WorkflowKey, result);
                _runner.Send(resultPacket);
            }
            catch (Exception ex)
            {
                // TODO Send Error Packet
                Logger.Instance.Error("SimpleContainer::ExecuteLocalDomain", ex);
            }
            finally
            {
                //_commandInstance = null;
            }
        }

        public void Send(BaseKey recipient, CommandMessage message)
        {
            var packet = new Packet(message.Key, recipient, message);
            _runner.Send(packet);
        }
    }
}
