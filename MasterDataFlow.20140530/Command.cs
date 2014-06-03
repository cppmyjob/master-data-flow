using MasterDataFlow._20140530.Interfaces;
using MasterDataFlow._20140530.Results;

namespace MasterDataFlow._20140530
{
    public abstract class Command<TCommandDataObject> : BaseCommand, ICommand<TCommandDataObject> 
        where TCommandDataObject : ICommandDataObject
    {

        public TCommandDataObject DataObject { get; internal set; }

        public abstract INextCommandResult<ICommandDataObject> Execute();

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

        internal protected override ICommandResult BaseExecute()
        {
            return Execute();
        }
    }
}
