using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using CommonDomain.Persistence;
using NEventStore;
using NEventStoreExample.Infrastructure;

namespace NEventStoreExample.Test
{
  public class InMemoryEventRepository : IRepository
  {
    private readonly IConstructAggregates aggregateFactory;

    private readonly List<IDomainEvent> givenEvents;

    public InMemoryEventRepository(List<IDomainEvent> givenEvents, IConstructAggregates aggregateFactory)
    {
      this.givenEvents = givenEvents;
      this.aggregateFactory = aggregateFactory;
    }

    public List<IDomainEvent> Events { get; private set; }

    public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregate
    {
      return this.GetById<TAggregate>(Bucket.Default, id, 0);
    }

    public TAggregate GetById<TAggregate>(Guid id, int version) where TAggregate : class, IAggregate
    {
      return this.GetById<TAggregate>(Bucket.Default, id, 0);
    }

    public TAggregate GetById<TAggregate>(string bucketId, Guid id) where TAggregate : class, IAggregate
    {
      return this.GetById<TAggregate>(Bucket.Default, id, 0);
    }

    public TAggregate GetById<TAggregate>(string bucketId, Guid id, int version) where TAggregate : class, IAggregate
    {
      var aggregate = this.aggregateFactory.Build(typeof(TAggregate), id, null);
      this.givenEvents.ForEach(aggregate.ApplyEvent);

      return (TAggregate)aggregate;
    }

    public void Save(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
    {
      this.Save(Bucket.Default, aggregate, commitId, updateHeaders);
    }

    public void Save(string bucketId, IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
    {
      this.Events = aggregate.GetUncommittedEvents().Cast<IDomainEvent>().ToList();
    }

    public void Dispose()
    {
      // no op
    }
  }
}