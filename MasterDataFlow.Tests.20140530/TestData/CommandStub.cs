using MasterDataFlow._20140530;
using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow.Tests._20140530.TestData
{
    public class CommandStub : Command<CommandDataObjectStub>
    {
        public override INextCommandResult<ICommandDataObject> Execute()
        {
            return NextStopCommand();
        }
    }
}
