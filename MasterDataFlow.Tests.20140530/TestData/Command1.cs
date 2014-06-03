using MasterDataFlow._20140530;
using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow.Tests._20140530.TestData
{
    public class Command1: Command<Command1DataObject>
    {
        public override INextCommandResult<ICommandDataObject> Execute()
        {
            return NextCommand<Command2>();
        }
    }
}
