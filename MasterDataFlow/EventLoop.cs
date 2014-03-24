using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    internal class EventLoop : IEventLoop
    {
        private AsyncQueue<BaseCommand> _queue = new AsyncQueue<BaseCommand>();

        public void Push(BaseCommand command)
        {
            _queue.Enqueue(command);
        }

        public void Loop()
        {
            if (_queue.Count == 0)
                return;
            var command = _queue.Dequeue();
            var result = command.BaseExecute();

            if (result == null)
            {
                // TODO Error
            }
            else
            {
                //var nextCommand = null;//result.FindNextCommand(_domain);
                //if (nextCommand != null)
                //{
                //    ICommandDataObject commandDataObject = null;
                //    var holder = result as IDataObjectHolder<ICommandDataObject>;
                //    if (holder != null)
                //    {
                //        commandDataObject = holder.DataObject;
                //    }
                //    //previousCommandInfo.OnChangeStatus(ExecuteStatus.Completed, null, commandDataObject);
                //}
                //else
                //{
                //    //Run(nextCommand.Definition, nextCommand.CommandDataObject, previousCommandInfo.OnChangeStatus);
                //}
            }
        }
    }
}
