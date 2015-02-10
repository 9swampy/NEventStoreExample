namespace NEventStoreExample.Infrastructure
{
  public interface IEventHandler<in TEvent>
  {
    void Handle(TEvent e);
  }
}