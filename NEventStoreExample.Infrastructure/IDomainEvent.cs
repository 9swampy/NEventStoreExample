namespace NEventStoreExample.Infrastructure
{
  using System;

  public interface IDomainEvent : IMessage
  {
    Guid ID { get; }

    int Version { get; }
  }
}