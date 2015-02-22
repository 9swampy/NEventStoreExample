namespace NEventStoreExample.Infrastructure.Bus
{
  public interface IEventPublisher
  {
    void Publish<T>(T @event) where T : NEventStoreExample.Infrastructure.IDomainEvent;
  }
}