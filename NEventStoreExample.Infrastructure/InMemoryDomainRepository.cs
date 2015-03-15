namespace NEventStoreExample.Infrastructure
{
  using CommonDomain.Core;
  using CommonDomain.Persistence;
  using CommonDomain.Persistence.EventStore;
  using NEventStore;

  public class InMemoryDomainRepository : EventStoreRepository, IEventStoreRepository
  {
    public InMemoryDomainRepository(IStoreEvents eventStore, IConstructAggregates aggregateFactory, ConflictDetector conflictDetector)
      : base(eventStore, aggregateFactory, conflictDetector)
    {
      this.EventStore = eventStore;
    }

    public InMemoryDomainRepository(IStoreEvents eventStore)
      : this(eventStore, new AggregateFactory(), new ConflictDetector())
    {      
    }

    public InMemoryDomainRepository()
      : this(Wireup.Init().UsingInMemoryPersistence().Build())
    { }

    public IStoreEvents EventStore
    {
      get;
      private set;
    }
  }
}
