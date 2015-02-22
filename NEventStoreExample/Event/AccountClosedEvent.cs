namespace NEventStoreExample.Event
{
  using System;

  [Serializable]
  public class AccountClosedEvent : DomainCorrelationEvent
  {
    public AccountClosedEvent(Guid id, int version, Guid correlationID, Guid causationID)
      : base(id, version, correlationID, causationID)
    {
    }
  }
}