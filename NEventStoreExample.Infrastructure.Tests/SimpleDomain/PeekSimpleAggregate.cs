namespace NEventStoreExample.Infrastructure.Tests.SimpleDomain
{
  using System;
  using NEventStoreExample.Infrastructure;

  public class PeekSimpleAggregate : Command
  {
    public PeekSimpleAggregate(Guid aggregateID, int originalVersion) : base(aggregateID, originalVersion)
    {
    }
  }
}