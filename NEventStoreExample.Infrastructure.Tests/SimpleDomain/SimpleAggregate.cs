namespace NEventStoreExample.Infrastructure.Tests.SimpleDomain
{
  using System;
  using CommonDomain;
  using CommonDomain.Core;
  using NEventStoreExample.Infrastructure;

  public class SimpleAggregate : AggregateBase, IMementoCreator
  {
    public SimpleAggregate(Guid aggregateID, DateTime lastUpdate) : this()
    {
      this.RaiseEvent(new SimpleAggregateCreated(aggregateID, lastUpdate));
    }

    private SimpleAggregate(SimpleAggregateMemento memento)
    {
      this.Id = memento.Id;
      this.Version = memento.Version;
      this.LastUpdate = memento.LastUpdate;
    }

    internal SimpleAggregate()
    {
      this.Register<SimpleAggregateCreated>(this.Apply);
      this.Register<SimpleAggregateUpdated>(this.Apply);
    }

    public DateTime LastUpdate { get; set; }

    public IMemento CreateMemento()
    {
      return new SimpleAggregateMemento(this.Id, this.Version, this.LastUpdate);
    }

    internal void Update()
    {
      this.RaiseEvent(new SimpleAggregateUpdated(this.Id, this.Version, DateTime.Now));
    }

    private void Apply(SimpleAggregateCreated domainEvent)
    {
      this.Id = domainEvent.AggregateID;
      this.LastUpdate = domainEvent.LastUpdate;
    }

    private void Apply(SimpleAggregateUpdated domainEvent)
    {
      this.LastUpdate = domainEvent.LastUpdate;
    }
  }
}