namespace NEventStoreExample.Event
{
  using System;
  using NEventStoreExample.Infrastructure;

  public abstract class DomainEvent : IDomainEvent
  {
    public DomainEvent(Guid id, int version)
    {
      this.ID = id;
      this.Version = version;
    }

    public Guid ID { get; private set; }

    public int Version { get; private set; }
  }
}