namespace NEventStoreExample.Infrastructure.Bus
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using LiteGuard;

  public class InProcessBus : RegistrationBusBase, IBus
  {
    private readonly DispatchStrategy dispatchStrategy;

    public InProcessBus(DispatchStrategy dispatchStrategy)
    {
      this.dispatchStrategy = dispatchStrategy;
    }

    public InProcessBus()
      : this(DispatchStrategy.Synchronous)
    {
    }

    public void Send<T>(T command) where T : ICommand
    {
      Dictionary<Guid, Action<IMessage>> handlers;

      if (this.routes.TryGetValue(typeof(T), out handlers) && handlers.Count > 0)
      {
        if (handlers.Count == 1)
        {
          handlers.Single().Value(command);
        }
        else if (handlers.Count > 1)
        {
          throw new InvalidOperationException(string.Format("Cannot send {0} to more than one handler", typeof(T).Name));
        }
      }
      else
      {
        throw new InvalidOperationException(string.Format("No handler registered for {0}", typeof(T).Name));
      }
    }

    public void Publish<T>(T @event) where T : IDomainEvent
    {
      Dictionary<Guid, Action<IMessage>> handlers;
      if (this.routes.TryGetValue(@event.GetType(), out handlers))
      {
        foreach (var handler in handlers)
        {
          if (this.dispatchStrategy == DispatchStrategy.Asynchronous)
          {
            ThreadPool.QueueUserWorkItem(x => handler.Value(@event));
          }
          else
          {
            handler.Value(@event);
          }
        }
      }
    }

    public void Dispose()
    {
      throw new NotImplementedException();
      //NOP
    }

    public override IDisposable Subscribe(object subscriber)
    {
      Guard.AgainstNullArgument("subscriber", subscriber);

      return this.RegisterAllHandlersByReflection(subscriber);
    }
  }
}