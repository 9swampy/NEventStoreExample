namespace NEventStoreExample.Domain.Event
{
  using System;

  public class AccountValidatedEvent : DomainCorrelationEvent
  {
    public AccountValidatedEvent(Guid id, int version, Guid correlationID, Guid causationID) : base(id, version, correlationID, causationID)
    {
    }
  }
}