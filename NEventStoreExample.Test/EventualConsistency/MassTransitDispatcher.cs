namespace NEventStoreExample.Test.EventualConsistency
{
  using System;
  using MemBus;
  using NEventStore;

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
        bus.Publish(@event.Body);
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