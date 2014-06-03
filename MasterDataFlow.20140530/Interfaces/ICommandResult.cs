namespace MasterDataFlow._20140530.Interfaces
{
    public interface ICommandResult
    {
        NextCommandResult FindNextCommand(CommandDomain domain);
    }
}
