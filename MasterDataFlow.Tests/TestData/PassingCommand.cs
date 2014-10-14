using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Tests.TestData
{
    public class PassingCommand : Command<PassingCommandDataObject>
    {
        public override BaseMessage Execute()
        {
            return Stop(new PassingCommandDataObject(DataObject.Id));
        }
    }
}
