using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Tests.TestData
{
    public class PassingCommandDataObject :  ICommandDataObject
    {
        public PassingCommandDataObject(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
