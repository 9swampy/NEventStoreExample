using System;
using NEventStoreExample.Event;
using NEventStoreExample.Infrastructure;

namespace NEventStoreExample.EventHandler
{
    // And, to show multiple event handlers in action, here's a handler that might
    // do something like send an email welcoming the person that just registered
    // or maybe a cool SignalR tie-in that goes to the sales dashboard
    // or a web service endpoint that has a Netduino polling it and ringing a gong when
    // someone signs up. You know, whatever.
    public class KaChingNotifier : IEventHandler<AccountCreatedEvent>
    {
        public void Handle(AccountCreatedEvent e)
        {
            Console.WriteLine("Dude, we got a customer, we're gonna be rich!");
        }
    }
}