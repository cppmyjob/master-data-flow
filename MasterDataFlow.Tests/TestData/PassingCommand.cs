using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Tests.TestData
{
    public class PassingCommand : Command<PassingCommandDataObject>
    {
        public object PassingCommandObject { get; set; }

        public override INextCommandResult<ICommandDataObject> Execute()
        {
            return NextStopCommand(new PassingCommandDataObject(DataObject.Id));
        }
    }
}
