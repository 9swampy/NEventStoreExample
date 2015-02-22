using System;
using MemBus;
using NEventStore;
using NEventStoreExample.Infrastructure.Bus;

namespace NEventStoreExample.Infrastructure
{
  // Ok, so this is where things get a little.... interesting
  // EventStore is sort of my coordinator for everything
  // When I create a new domain object, I issue commands to it. It, in turn, raises events to change its internal state.
  //
  // Again, thing of the current state of a domain object as what you get after all events that built it are applied.
  // new Person("@benhyr") might raise a PersonCreatedEvent. Then person.UpdateTwitterId("@hyrmn") raises a PersonUpdatedEvent
  // When I load that Person from the EventStore, rather than getting Person.TwitterId from a db field, I'm getting PersonCreatedEvent
  // (which sets the TwitterId initially) and then PersonUpdatedEvent (which updates the TwitterId to the new value)
  //
  // Now, back to this class. When I raise events, they're uncommitted until I persist them back to the EventStore.
  // By default, we assume others might be interested in these uncommitted events. Of course, it's EventStore not EventStoreAndMessageBus
  // (although EventStore could do some basic stuff for us). So, we're telling EventStore to publish to our MemBus bus... at some point,
  // we might put NSB or MassTransit or EasyNetQ or whatever in place.
  public static class DelegateDispatcher
  {
    public static void DispatchCommit(IEventPublisher bus, ICommit commit)
    {
      // This is where we'd hook into our messaging infrastructure, such as NServiceBus,
      // MassTransit, WCF, or some other communications infrastructure.
      // This can be a class as well--just implement IDispatchCommits.
      foreach (var @event in commit.Events)
      {
        System.Diagnostics.Debug.Print(string.Format("Dispatch {0}", @event.Body.GetType().Name));
        if (@event.Body is IDomainEvent)
        {
          bus.Publish(@event.Body as IDomainEvent);
        }
        else
        {
          throw new NotImplementedException();
        }        
      }
    }
  }
}