using MasterDataFlow._20140530;
using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow.Tests._20140530.TestData
{
    public class Command2: Command<Command2DataObject>
    {
        public override INextCommandResult<ICommandDataObject> Execute()
        {
            return NextStopCommand();
        }
    }
}
