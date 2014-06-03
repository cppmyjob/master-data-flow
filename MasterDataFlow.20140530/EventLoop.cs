using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow._20140530
{
    internal class EventLoop : IEventLoop
    {
        private readonly AsyncQueue<BaseCommand> _queue = new AsyncQueue<BaseCommand>();

        internal AsyncQueue<BaseCommand> Queue
        {
            get { return _queue; }
        }

        public void Push(BaseCommand command)
        {
            _queue.Enqueue(command);
        }

        public void Loop()
        {
            if (_queue.Count == 0)
                return;
            var command = _queue.Dequeue();
            ICommandResult result = command.BaseExecute();

            if (result == null)
            {
                // TODO Error
            }
            else
            {
                //result.FindNextCommand()
                //_queue.Enqueue(result);
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