namespace NEventStoreExample.Infrastructure.Tests.SimpleDomain
{
  using System;
  using System.Collections.Generic;
  using CommonDomain;

  public class SimpleAggregateMemento : IMemento
  {
    public SimpleAggregateMemento(Guid id, int version, DateTime lastUpdate)
    {
      this.Dictionary = new Dictionary<string, Guid>();
      this.Id = id;
      this.Version = version;
      this.LastUpdate = lastUpdate;
    }

    public Dictionary<string, Guid> Dictionary { get; private set; }

    public Guid Id { get; set; }

    public int Version { get; set; }

    public DateTime LastUpdate { get; set; }
  }
}