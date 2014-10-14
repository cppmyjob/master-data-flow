using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Tests.TestData
{
    public class Command1: Command<Command1DataObject>
    {
        public override BaseMessage Execute()
        {
            //return NextCommand<Command2>();
            return null;
        }
    }
}
