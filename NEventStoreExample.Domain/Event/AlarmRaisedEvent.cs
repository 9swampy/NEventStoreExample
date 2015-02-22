namespace NEventStoreExample.Domain.Event
{
  using System;

  public class AlarmRaisedEvent : DomainCorrelationEvent
  {
    public AlarmRaisedEvent(Guid id, int version, Guid correlationID, Guid causationID) : base(id, version, correlationID, causationID)
    {
    }
  }
}