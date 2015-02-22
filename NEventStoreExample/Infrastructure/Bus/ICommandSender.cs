namespace NEventStoreExample.Infrastructure.Bus
{
  public interface ICommandSender
  {
    void Send<T>(T command) where T : NEventStoreExample.Infrastructure.ICommand;
  }
}