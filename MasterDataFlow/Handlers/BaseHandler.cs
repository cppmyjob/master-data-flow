using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces.Network;

namespace MasterDataFlow.Handlers
{
    public abstract class BaseHandler
    {
        private IHub _parent;

        public abstract string[] SupportedActions { get; }

        public IHub Parent
        {
            get { return _parent; }
        }

        public abstract void Execute(string actionName, IPacket packet);

        internal protected virtual void ConnectHub(IHub hub)
        {
            
        }

        internal protected virtual void RegisterParentHub(IHub hub)
        {
            if (Parent != null)
                throw new MasterDataFlowException("Parent already exists");
            _parent = hub;
        }
    }
}
