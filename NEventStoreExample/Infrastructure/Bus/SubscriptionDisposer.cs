namespace NEventStoreExample.Infrastructure.Bus
{
  using System;
  using System.Collections.Generic;

  public class SubscriptionDisposer : IDisposable
  {
    private readonly InProcessBus bus;
    private IDictionary<Guid, Type> handlerList;
    
    public SubscriptionDisposer(InProcessBus bus)
    {
      this.bus = bus;
    }

    public void AddHandler(Type messageTypeHandled, Guid handlerID)
    {
      this.HandlerList.Add(handlerID, messageTypeHandled);
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

    public void Dispose()
    {
      foreach (KeyValuePair<Guid, Type> handler in this.HandlerList)
      {
        bus.DeregisterHandler(handler.Value, handler.Key);
      }
    }
  }
}