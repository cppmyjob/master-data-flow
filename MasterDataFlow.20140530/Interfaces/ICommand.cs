namespace MasterDataFlow._20140530.Interfaces
{
    public interface ICommand<out TCommandDataObject> : IDataObjectHolder<TCommandDataObject> 
        where TCommandDataObject : ICommandDataObject
    {
        INextCommandResult<ICommandDataObject> Execute();
    }
}
