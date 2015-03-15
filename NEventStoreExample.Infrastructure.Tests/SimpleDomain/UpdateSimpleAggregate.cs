namespace NEventStoreExample.Infrastructure.Tests.SimpleDomain
{
  using System;
  using NEventStoreExample.Infrastructure;

  public class UpdateSimpleAggregate : Command
  {
    public UpdateSimpleAggregate(Guid aggregateID, int originalVersion, DateTime lastUpdate)
      : base(aggregateID, originalVersion)
    {
      this.LastUpdate = lastUpdate;
    }

    public DateTime LastUpdate { get; set; }
  }
}