using System;
using CommonDomain.Core;
using NEventStoreExample.Event;

namespace NEventStoreExample.Model
{
    // This is my old buddy Account. It inherits from AggregateBase, which comes from CommonDomain.
    // There's no real need to bring CommonDomain in if you don't want. It provides a couple simple mechanisms for me.
    // First, it gives me the IRepository wrapper around EventStore which I use above in my CommandHandlers
    // Second, it gives me a base that tracks all of my uncommitted changes for me.
    // Third, it wires up, by convention, my event handlers (the private void Apply(SomeEvent @event) methods
    public class Account : AggregateBase
    {
        public Account(AccountId id, string name, string twitter) : this(id.Value)
        {
            RaiseEvent(new AccountCreatedEvent(Id.Value, name, twitter, true));
        }

        // Aggregate should have only one public constructor
        private Account(Guid id)
        {
            Id = new AccountId(id);
            base.Id = id;
        }

        public new AccountId Id { get; private set; }

        public string Name { get; private set; }

        public string Twitter { get; private set; }

        public bool IsActive { get; private set; }

        public void Close()
        {
            RaiseEvent(new AccountClosedEvent());
        }

        private void Apply(AccountCreatedEvent @event)
        {
            Id = new AccountId(@event.Id);
            Name = @event.Name;
            Twitter = @event.Twitter;
            IsActive = @event.IsActive;
        }

        private void Apply(AccountClosedEvent e)
        {
            IsActive = false;
        }
    }
}