namespace NEventStoreExample.Infrastructure
{
  public interface IEventHandler<in TEvent>
    where TEvent : IDomainEvent
  {
    void Handle(TEvent e);
  }
}