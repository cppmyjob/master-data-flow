using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Actions;
using MasterDataFlow.Handlers;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Utils;

namespace MasterDataFlow.Network
{
    public class ActionHub : EventLoopHub
    {
        private readonly ServiceKey _key = new ServiceKey();
        private readonly AsyncDictionary<string, BaseHandler> _handlers = new AsyncDictionary<string, BaseHandler>();
        private readonly List<BaseHandler> _unuqueHandlers = new List<BaseHandler>();

        public ActionHub()
        {
        }

        public override BaseKey Key
        {
            get { return _key; }
        }

        public override bool ConnectHub(IHub hub)
        {
            var result = base.ConnectHub(hub);
            foreach (var handler in _unuqueHandlers)
            {
                handler.ConnectHub(hub);
            }
            return result;
        }

        protected void RegisterHandler(BaseHandler handler)
        {
            handler.RegisterParentHub(this);
            foreach (var supportedAction in handler.SupportedActions)
            {
                _handlers.AddItem(supportedAction, handler);
            }
            _unuqueHandlers.Add(handler);
        }

        protected override void ProccessPacket(IPacket packet)
        {
            if (packet.Body == null)
                // TODO Exception?
                return;
            var action = packet.Body as BaseAction;
            if (action == null)
                // TODO Exception?
                return;
            var handler = _handlers.GetItem(action.Name);
            if (handler == null)
                // TODO Exception?
                return;
            handler.Execute(action.Name, packet);
        }

    }
}
