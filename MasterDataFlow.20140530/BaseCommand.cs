using MasterDataFlow._20140530.Interfaces;

namespace MasterDataFlow._20140530
{
    public abstract class BaseCommand 
    {
        internal protected abstract ICommandResult BaseExecute();
    }
}
