namespace NEventStoreExample.Infrastructure.Bus
{
  using System;
  using System.Collections.Generic;

  public class SubscriptionDisposer : IDisposable
  {
    private readonly RegistrationBusBase bus;
    private IDictionary<Guid, Type> handlerList;

    public SubscriptionDisposer(RegistrationBusBase bus)
    {
      this.bus = bus;
    }

    private IDictionary<Guid, Type> HandlerList
    {
      get
      {
        if (this.handlerList == null)
        {
          this.handlerList = new Dictionary<Guid, Type>();
        }
        return this.handlerList;
      }
    }

    public void AddHandler(Type messageTypeHandled, Guid handlerID)
    {
      this.HandlerList.Add(handlerID, messageTypeHandled);
    }

    public void Dispose()
    {
      foreach (KeyValuePair<Guid, Type> handler in this.HandlerList)
      {
        this.bus.DeregisterHandler(handler.Value, handler.Key);
      }
    }
  }
}