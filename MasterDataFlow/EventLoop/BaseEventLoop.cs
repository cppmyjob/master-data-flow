﻿using System;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Utils;

namespace MasterDataFlow.EventLoop
{

    public delegate void EventLoopCallback(Guid loopId, EventLoopCommandStatus status, ILoopCommandMessage message = null);

    public class BaseEventLoop
    {
        internal class LoopItem
        {
            public LoopItem()
            {
                Id = Guid.NewGuid();
                Status = EventLoopCommandStatus.NotStarted;
            }
            // TODO Need to think about Guid. But it's easest way now
            public Guid Id { get; set; }
            public ILoopCommand Command { get; set; }
            public EventLoopCommandStatus Status { get; set; }
            public EventLoopCallback InputCallback { get; set; }
        }

        private class CallbackCommand : ILoopCommand
        {
            private readonly BaseEventLoop _eventLoop;
            private readonly Guid _id;
            private readonly EventLoopCommandStatus _status;
            private readonly ILoopCommandMessage _message;

            public CallbackCommand(BaseEventLoop eventLoop, Guid id, EventLoopCommandStatus status, ILoopCommandMessage message)
            {
                _eventLoop = eventLoop;
                _id = id;
                _status = status;
                _message = message;
            }

            public void Execute(Guid loopId, ILoopCommandData data, EventLoopCallback callback)
            {
                // TODO need check orders
                _eventLoop._commandWaiting.RemoveItem(loopId);
                var item = _eventLoop._commandWaiting.GetItem(_id);
                if (item == null)
                {
                    throw new Exception("!!!! 1");
                }

                switch (_status)
                {
                    case EventLoopCommandStatus.Completed:
                    case EventLoopCommandStatus.Fault:
                        if (item.InputCallback != null)
                        {
                            // TODO Is need try catch?
                            item.InputCallback(_id, _status, _message);
                            _eventLoop._commandWaiting.RemoveItem(_id);
                        }
                        break;
                    case EventLoopCommandStatus.Progress:
                        if (item.InputCallback != null)
                        {
                            // TODO Is need try catch?
                            item.InputCallback(_id, _status, _message);
                        }
                        break;
                    default:
                        throw new Exception("!!!! 2");
                        break;
                }
            }
        }

        private readonly AsyncQueue<LoopItem> _queue = new AsyncQueue<LoopItem>();
        private readonly AsyncDictionary<Guid, LoopItem> _commandWaiting = new AsyncDictionary<Guid, LoopItem>();

        internal AsyncQueue<LoopItem> Queue
        {
            get { return _queue; }
        }

        protected internal Guid Push(ILoopCommand command, EventLoopCallback callback = null)
        {
            var item = new LoopItem
            {
                Command = command,
                InputCallback = callback
            };
            _queue.Enqueue(item);
            return item.Id;
        }

        protected internal bool Loop()
        {
            if (_queue.Count == 0)
                return false;
            var item = _queue.Dequeue();
            _commandWaiting.AddItem(item.Id, item);
            item.Command.Execute(item.Id, null, WaitingCallback);
            return true;
        }

        private void WaitingCallback(Guid id, EventLoopCommandStatus status, ILoopCommandMessage message)
        {
            var callbackCommand = new CallbackCommand(this, id, status, message);
            Push(callbackCommand);
        }
    }
}