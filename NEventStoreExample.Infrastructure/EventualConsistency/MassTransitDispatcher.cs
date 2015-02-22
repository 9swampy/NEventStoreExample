namespace NEventStoreExample.Infrastructure.EventualConsistency
{
  using System;
  using NEventStore;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;

  internal class MassTransitDispatcher : IObserver<ICommit>
  {
    private readonly IBus bus;

    public MassTransitDispatcher(IBus bus)
    {
      this.bus = bus;
    }

    public void Dispose()
    {
    }

    public void OnNext(ICommit commit)
    {
      foreach (var @event in commit.Events)
      {
        if (@event.Body is IDomainEvent)
        {
          this.bus.Publish(@event.Body as IDomainEvent);
        }
        else
        {
          throw new NotImplementedException();
        }
      }
    }

    public void OnError(Exception error)
    {
      // ...
    }

    public void OnCompleted()
    {
    }
  }
}