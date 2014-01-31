using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Tests.TestData
{
    public class Command1: Command<Command1DataObject>
    {
        public override INextCommandResult<ICommandDataObject> Execute()
        {
            return NextCommand<Command2>();
        }
    }
}
