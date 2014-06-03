namespace MasterDataFlow._20140530.Interfaces
{
    internal interface IEventLoop
    {
        void Push(BaseCommand command);
    }
}
