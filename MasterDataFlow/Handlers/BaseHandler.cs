using MasterDataFlow.Interfaces.Network;

namespace MasterDataFlow.Handlers
{
    public abstract class BaseHandler
    {
        public abstract string[] SupportedActions { get; }
        public abstract void Execute(string actionName, IPacket packet);

        public virtual void ConnectHub(IHub hub)
        {
            
        }
    }
}
