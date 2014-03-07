using System;
using NEventStoreExample.Infrastructure;

namespace NEventStoreExample.Event
{
    // This is going to seem a bit conflated so bear with me. When we create a new Account,
    // we raise an AccountCreatedEvent. We then apply that AccountCreatedEvent to ourselves.
    // Once we save our uncommitted events to EventStore, then that AccountCreatedEvent is also
    // sent out to our bus for other interested parties.
    [Serializable]
    public class AccountCreatedEvent : IEvent
    {
        public AccountCreatedEvent(Guid id, string name, string twitter, bool isActive)
        {
            Id = id;
            Name = name;
            Twitter = twitter;
            IsActive = isActive;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Twitter { get; private set; }

        public bool IsActive { get; private set; }
    }
}