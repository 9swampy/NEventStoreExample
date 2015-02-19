namespace NEventStoreExample.Event
{
  using System;

  [Serializable]
  public class AccountClosedEvent : DomainEvent
  {
    public AccountClosedEvent(Guid id, int version)
      : base(id, version)
    {
    }
  }
}