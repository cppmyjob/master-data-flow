                                                                                            using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;

namespace MasterDataFlow
{
    public abstract class Command<TCommandDataObject> : BaseCommand, ICommand<TCommandDataObject> 
        where TCommandDataObject : ICommandDataObject
    {

        public TCommandDataObject DataObject { get; internal set; }

        public abstract BaseMessage Execute();

        protected BaseMessage Stop(ICommandDataObject dataObject)
        {
            return new StopCommandMessage(Key, dataObject);
        }

        internal protected override BaseMessage BaseExecute()
        {
            return Execute();
        }
    }
}
