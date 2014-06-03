namespace MasterDataFlow._20140530.Interfaces
{
    public interface IDataObjectHolder<out TCommandDataObject>
        where TCommandDataObject : ICommandDataObject
    {
        TCommandDataObject DataObject { get; }
    }
}
