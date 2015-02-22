namespace NEventStoreExample.Domain.Event
{
  using System;

  public abstract class DomainCorrelationEvent : DomainEvent
  {
    private readonly Guid correlationID;
    private readonly Guid causationID;

    public DomainCorrelationEvent(Guid id, int version, Guid correlationID, Guid causationID) : base(id,version)
    {
      this.correlationID = correlationID;
      this.causationID = causationID;
    }

    public Guid CorrelationID
    {
      get
      {
        return this.correlationID;
      }
    }

    public Guid CausationID
    {
      get
      {
        return this.causationID;
      }
    }
  }
}