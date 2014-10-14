using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Tests.TestData
{
    public class Command2: Command<Command2DataObject>
    {
        public override BaseMessage Execute()
        {
            //return NextStopCommand();
            return null;
        }
    }
}
