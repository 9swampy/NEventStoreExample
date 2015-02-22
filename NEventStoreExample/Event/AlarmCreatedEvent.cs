namespace NEventStoreExample.Event
{
  using System;

  public class AlarmCreatedEvent : DomainCorrelationEvent
  {
    public AlarmCreatedEvent(Guid id, int version, Guid correlationID, Guid causationID, int seconds)
      : base(id, version, correlationID, causationID)
    {
      this.Seconds = seconds;
    }

    public int Seconds { get; set; }
  }
}
