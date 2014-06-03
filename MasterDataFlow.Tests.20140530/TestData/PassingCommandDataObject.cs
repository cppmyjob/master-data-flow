using System;
using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow.Tests._20140530.TestData
{
    public class PassingCommandDataObject :  ICommandDataObject
    {
        public PassingCommandDataObject(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
