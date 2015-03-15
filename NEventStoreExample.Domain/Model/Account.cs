using System;
using CommonDomain.Core;
using NEventStoreExample.Domain.Event;

namespace NEventStoreExample.Domain.Model
{
  // This is my old buddy Account. It inherits from AggregateBase, which comes from CommonDomain.
  // There's no real need to bring CommonDomain in if you don't want. It provides a couple simple mechanisms for me.
  // First, it gives me the IRepository wrapper around EventStore which I use above in my CommandHandlers
  // Second, it gives me a base that tracks all of my uncommitted changes for me.
  // Third, it wires up, by convention, my event handlers (the private void Apply(SomeEvent @event) methods
  public class Account : AggregateBase
  {
    public Account(Guid id, string name, string twitter)
      :this()
    {
      this.RaiseEvent(new AccountCreatedEvent(id, 0, name, twitter, true));
    }

    // Aggregate should have only one public constructor
    private Account()
    {
      this.Register<AccountCreatedEvent>(this.Apply);
      this.Register<AccountClosedEvent>(this.Apply);
    }

    public string Name { get; private set; }

    public string Twitter { get; private set; }

    public bool IsActive { get; private set; }

    public void Close()
    {
      this.RaiseEvent(new AccountClosedEvent(this.Id, this.Version, Guid.NewGuid(), Guid.Empty));
    }

    private void Apply(AccountCreatedEvent @event)
    {
      this.Id = @event.AggregateID;
      this.Name = @event.Name;
      this.Twitter = @event.Twitter;
      this.IsActive = @event.IsActive;
    }

    private void Apply(AccountClosedEvent e)
    {
      this.IsActive = false;
    }
  }
}