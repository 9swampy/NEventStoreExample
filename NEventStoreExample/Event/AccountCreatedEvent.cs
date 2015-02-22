namespace NEventStoreExample.Event
{
  using System;

  // This is going to seem a bit conflated so bear with me. When we create a new Account,
  // we raise an AccountCreatedEvent. We then apply that AccountCreatedEvent to ourselves.
  // Once we save our uncommitted events to EventStore, then that AccountCreatedEvent is also
  // sent out to our bus for other interested parties.
  [Serializable]
  public class AccountCreatedEvent : DomainEvent
  {
    public AccountCreatedEvent(Guid id, int version, string name, string twitter, bool isActive)
      : base(id, version)
    {
      this.Name = name;
      this.Twitter = twitter;
      this.IsActive = isActive;
    }

    public string Name { get; private set; }

    public string Twitter { get; private set; }

    public bool IsActive { get; private set; }
  }
}