namespace NEventStoreExample.Infrastructure.Bus
{
  using System;

  public class MemBusAdapter : IBus
  {
    private readonly MemBus.IBus bus;

    public MemBusAdapter(MemBus.IBus bus)
    {
      this.bus = bus;
    }

    public void Dispose()
    {
      this.bus.Dispose();
    }

    public void Publish<T>(T @event) where T : IDomainEvent
    {
      this.bus.Publish(@event);
    }

    public void Send<T>(T command) where T : ICommand
    {
      this.bus.Publish(command);
    }

    public IDisposable Subscribe(object subscriber)
    {
      return this.bus.Subscribe(subscriber);
    }

    public IDisposable Subscribe<M>(Action<M> subscription)
    {
      return this.bus.Subscribe<M>(subscription);
    }
  }
}