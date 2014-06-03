using MasterDataFlow._20140530;
using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow.Tests._20140530.TestData
{
    public class PassingCommand : Command<PassingCommandDataObject>
    {
        public override INextCommandResult<ICommandDataObject> Execute()
        {
            return NextStopCommand(new PassingCommandDataObject(DataObject.Id));
        }
    }
}
