namespace NEventStoreExample.Event
{
  using System;

  public class AccountInvalidatedEvent : DomainCorrelationEvent
  {
    public AccountInvalidatedEvent(Guid id, int version, Guid correlationID, Guid causationID)
      : base(id, version, correlationID, causationID)
    {
    }
  }
}