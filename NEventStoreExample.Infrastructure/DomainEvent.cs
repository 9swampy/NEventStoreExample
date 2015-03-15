namespace NEventStoreExample.Infrastructure
{
  using System;
  
  public abstract class DomainEvent : IDomainEvent
  {
    public DomainEvent(Guid id, int version)
    {
      this.AggregateID = id;
      this.OriginalVersion = version;
    }

    public Guid AggregateID { get; set; }

    public int OriginalVersion { get; set; }
  }
}