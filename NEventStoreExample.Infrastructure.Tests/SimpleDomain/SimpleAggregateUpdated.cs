namespace NEventStoreExample.Infrastructure.Tests.SimpleDomain
{
  using System;
  using System.Linq;
  using NEventStoreExample.Infrastructure;

  public class SimpleAggregateUpdated : DomainEvent
  {
    public SimpleAggregateUpdated(Guid aggregateID, int originalVersion, DateTime lastUpdate) : base(aggregateID, originalVersion)
    {
      this.LastUpdate = lastUpdate;
    }

    public DateTime LastUpdate { get; set; }
  }
}