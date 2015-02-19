namespace NEventStoreExample.Infrastructure
{
  using System;

  public interface IDomainEvent
  {
    Guid ID { get; }

    int Version { get; }
  }
}