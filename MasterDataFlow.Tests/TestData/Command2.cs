using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Tests.TestData
{
    public class Command2: Command<Command2DataObject>
    {
        public override INextCommandResult<ICommandDataObject> Execute()
        {
            return NextStopCommand();
        }
    }
}
