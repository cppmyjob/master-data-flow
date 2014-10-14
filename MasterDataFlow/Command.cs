using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;
using MasterDataFlow.Results;

namespace MasterDataFlow
{
    public abstract class Command<TCommandDataObject> : BaseCommand, ICommand<TCommandDataObject> 
        where TCommandDataObject : ICommandDataObject
    {

        public TCommandDataObject DataObject { get; internal set; }

        public abstract BaseMessage Execute();

        protected BaseMessage Stop(ICommandDataObject dataObject)
        {
            return new StopCommandMessage(Key, dataObject);
        }

        protected INextCommandResult<TCommandDataObject> NextStopCommand()
        {
            return NextStopCommand<TCommandDataObject>();
        }

        protected INextCommandResult<TNextCommandDataObject> NextStopCommand<TNextCommandDataObject>(TNextCommandDataObject dataObject = default(TNextCommandDataObject))
            where TNextCommandDataObject : ICommandDataObject
        {
            return new NextStopCommandResult<TNextCommandDataObject>(dataObject);
        }

        protected INextCommandResult<ICommandDataObject> NextCommand<TNextCommand>(ICommandDataObject dataObject = null)
            where TNextCommand : ICommand<ICommandDataObject>
        {
            return new NextTypeCommandResult<TNextCommand>(dataObject);
        }

        internal protected override BaseMessage BaseExecute()
        {
            return Execute();
        }
    }
}
