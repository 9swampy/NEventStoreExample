namespace NEventStoreExample.Infrastructure.Tests.SimpleDomain
{
  using System;
  using NEventStoreExample.Infrastructure;

  public class SimpleAggregateCreated : DomainEvent
  {
    public SimpleAggregateCreated(Guid universeID, DateTime lastUpdate)
      :base(universeID,0)
    {
      this.OriginalVersion = 0;
      this.LastUpdate = lastUpdate;
    }

    public DateTime LastUpdate { get; set; }
  }
}