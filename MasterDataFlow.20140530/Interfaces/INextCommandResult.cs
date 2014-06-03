namespace MasterDataFlow._20140530.Interfaces
{

    public interface INextCommandResult<out TCommandDataObject> : ICommandResult, IDataObjectHolder<TCommandDataObject>
        where TCommandDataObject : ICommandDataObject
    {
    }
}
