using System;
using NEventStoreExample.Infrastructure;

namespace NEventStoreExample.Event
{
    [Serializable]
    public class AccountClosedEvent : IEvent
    {
    }
}