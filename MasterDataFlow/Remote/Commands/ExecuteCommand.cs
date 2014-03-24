using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Remote.Commands
{
    public class ExecuteCommand : RequestCommand<ExecuteCommand.Params>
    {
        public class Params : RequestParams
        {
            public Guid DomainId { get; set; }
            public string CommandTypeName { get; set; }
            public string DataObjectTypeName { get; set; }
            public string DataObject { get; set; }
        }

        internal ExecuteCommand(IRemoteContext remoteContext, Guid requestId, Guid domainId, string commandTypeName, string dataObjectTypeName, string dataObject) :
            base(remoteContext)
        {
            DataObject = new Params
            {
                RequestId = requestId,
                DomainId = domainId,
                CommandTypeName = commandTypeName,
                DataObjectTypeName = dataObjectTypeName,
                DataObject = dataObject
            };
        }

        public override INextCommandResult<ICommandDataObject> Execute()
        {
            throw new NotImplementedException();

            //var domainInstance = _domainResolver.Find(domainId);
            //var commandType = Type.GetType(commandTypeName, false);
            //if (commandType == null)
            //    throw new RemoteException(String.Format("Can't load command type {0}", commandTypeName));
            //var dataObjectType = Type.GetType(dataObjectTypeName, false);
            //if (dataObjectType == null)
            //    throw new RemoteException(String.Format("Can't load data object type {0}", dataObjectTypeName));
            //var dataObjectInstance = (ICommandDataObject)Deserialization.DeserializateDataObject(dataObjectType, dataObject);
            //var context = domainInstance.Start(commandType, dataObjectInstance);
        }
    }
}
