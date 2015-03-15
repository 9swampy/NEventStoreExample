namespace NEventStoreExample.Infrastructure.Tests
{
  using System;
  using CommonDomain;
  using CommonDomain.Persistence;
  using FakeItEasy;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStore;
  using NEventStoreExample.Infrastructure.Tests.SimpleDomain;

  [TestClass]
  public class SimpleSnapshot
  {
    [TestMethod]
    public void SimpleAggregateDirectSnapshotting()
    {
      IStoreEvents memoryStore = Wireup.Init().UsingInMemoryPersistence().Build();
      IConstructAggregates aggregateFactory = A.Fake<IConstructAggregates>();
      IEventStoreRepository repository = new InMemoryDomainRepository(memoryStore, aggregateFactory, new CommonDomain.Core.ConflictDetector());
      Guid aggregateID = Guid.NewGuid();
      SimpleAggregate aggregate = new SimpleAggregate(aggregateID, DateTime.Now);
      repository.Save(aggregate, Guid.NewGuid());
      IMemento memento = aggregate.CreateMemento();
      ISnapshot snapshot = new Snapshot(aggregate.Id.ToString(), aggregate.Version, memento);

      repository.EventStore.Advanced.AddSnapshot(snapshot);

      ISnapshot retrievedSnapshot = repository.EventStore.Advanced.GetSnapshot(aggregate.Id, int.MaxValue);

      retrievedSnapshot.Should().NotBeNull();
      retrievedSnapshot.StreamRevision.Should().Be(1);
      retrievedSnapshot.ShouldBeEquivalentTo(snapshot);

      repository.GetById<SimpleAggregate>(aggregate.Id);

      A.CallTo(() => aggregateFactory.Build(typeof(SimpleAggregate), aggregate.Id, A<IMemento>.That.Not.IsNull())).MustHaveHappened();
    }

    [TestMethod]
    public void SimpleAggregateSnapshottingViaSnapshotCreatorAndConcreteAggregateFactory()
    {
      IStoreEvents memoryStore = Wireup.Init().UsingInMemoryPersistence().Build();
      IConstructAggregates aggregateFactory = new AggregateFactory();
      IEventStoreRepository repository = new InMemoryDomainRepository(memoryStore, aggregateFactory, new CommonDomain.Core.ConflictDetector());
      Guid aggregateID = Guid.NewGuid();
      SimpleAggregate aggregate = new SimpleAggregate(aggregateID, DateTime.Now);
      repository.Save(aggregate, Guid.NewGuid());
      aggregate.Version.Should().Be(1);

      SnapshotCreator<SimpleAggregate> snapshotCreator = new SnapshotCreator<SimpleAggregate>(repository, 1);
      ISnapshot snapshot = snapshotCreator.SaveSnapShot(aggregate.Id);

      ISnapshot retrievedSnapshot = repository.EventStore.Advanced.GetSnapshot(aggregate.Id, int.MaxValue);

      retrievedSnapshot.Should().NotBeNull();
      retrievedSnapshot.StreamRevision.Should().Be(1);
      retrievedSnapshot.ShouldBeEquivalentTo(snapshot);

      SimpleAggregate retrievedAggregate = repository.GetById<SimpleAggregate>(aggregate.Id);

      retrievedAggregate.Should().NotBeNull();
      aggregate.Version.Should().Be(1);
      retrievedAggregate.ShouldBeEquivalentTo(aggregate);
    }

    [TestMethod]
    public void SimpleAggregateHasSnapshotBeforeSnapshotting()
    {
      IStoreEvents memoryStore = Wireup.Init().UsingInMemoryPersistence().Build();
      IConstructAggregates aggregateFactory = new AggregateFactory();
      IEventStoreRepository repository = new InMemoryDomainRepository(memoryStore, aggregateFactory, new CommonDomain.Core.ConflictDetector());
      Guid aggregateID = Guid.NewGuid();
      SimpleAggregate aggregate = new SimpleAggregate(aggregateID, DateTime.Now);
      repository.Save(aggregate, Guid.NewGuid());
      aggregate.Version.Should().Be(1);

      ISnapshot retrievedSnapshot = repository.EventStore.Advanced.GetSnapshot(aggregate.Id, int.MaxValue);

      retrievedSnapshot.Should().BeNull();
    }

    [Ignore]
    [TestMethod]
    public void SimpleAggregateSnapshottingViaSnapshotCreator()
    {
      IStoreEvents memoryStore = Wireup.Init().UsingInMemoryPersistence().Build();
      IConstructAggregates aggregateFactory = A.Fake<IConstructAggregates>();
      IEventStoreRepository repository = new InMemoryDomainRepository(memoryStore, aggregateFactory, new CommonDomain.Core.ConflictDetector());
      Guid aggregateID = Guid.NewGuid();
      SimpleAggregate aggregate = new SimpleAggregate(aggregateID, DateTime.Now);
      repository.Save(aggregate, Guid.NewGuid());

      A.CallTo(() => aggregateFactory.Build(typeof(SimpleAggregate), aggregate.Id, null)).ReturnsLazily(() => new SimpleAggregate());

      SnapshotCreator<SimpleAggregate> snapshotCreator = new SnapshotCreator<SimpleAggregate>(repository, 1);
      Snapshot snapshot = snapshotCreator.SaveSnapShot(aggregate.Id);
      A.CallTo(() => aggregateFactory.Build(typeof(SimpleAggregate), aggregate.Id, A<IMemento>.That.IsNull())).MustHaveHappened(Repeated.Exactly.Once);

      ISnapshot retrievedSnapshot = repository.EventStore.Advanced.GetSnapshot(aggregate.Id, int.MaxValue);
      A.CallTo(() => aggregateFactory.Build(typeof(SimpleAggregate), aggregate.Id, A<IMemento>.That.IsNull())).MustHaveHappened(Repeated.Exactly.Once);

      retrievedSnapshot.Should().NotBeNull();
      retrievedSnapshot.StreamRevision.Should().Be(1);
      retrievedSnapshot.ShouldBeEquivalentTo(snapshot);

      SimpleAggregate retrievedAggregate = repository.GetById<SimpleAggregate>(aggregate.Id);
      retrievedAggregate.Should().NotBeNull();
      aggregate.Version.Should().Be(1);
      retrievedAggregate.ShouldBeEquivalentTo(aggregate);
      A.CallTo(aggregateFactory).Where(o => o.Method.Name != "Build").MustNotHaveHappened();
      A.CallTo(() => aggregateFactory.Build(typeof(SimpleAggregate), aggregate.Id, A<IMemento>.That.IsNull())).MustHaveHappened(Repeated.Exactly.Once);

      A.CallTo(() => aggregateFactory.Build(typeof(SimpleAggregate), aggregate.Id, A<IMemento>.That.Not.IsNull())).MustHaveHappened();
    }

    [TestMethod]
    public void SimpleAggregateRetrieval()
    {
      IStoreEvents memoryStore = Wireup.Init().UsingInMemoryPersistence().Build();
      IConstructAggregates aggregateFactory = A.Fake<IConstructAggregates>();
      IEventStoreRepository repository = new InMemoryDomainRepository(memoryStore, aggregateFactory, new CommonDomain.Core.ConflictDetector());
      Guid aggregateID = Guid.NewGuid();
      SimpleAggregate aggregate = new SimpleAggregate(aggregateID, DateTime.Now);
      repository.Save(aggregate, Guid.NewGuid());

      A.CallTo(() => aggregateFactory.Build(typeof(SimpleAggregate), aggregate.Id, null)).Returns(new SimpleAggregate(aggregateID, DateTime.Now));

      repository.GetById<SimpleAggregate>(aggregate.Id);

      A.CallTo(aggregateFactory).MustHaveHappened();
      A.CallTo(aggregateFactory).Where(o => o.Method.Name != "Build").MustNotHaveHappened();
      A.CallTo(() => aggregateFactory.Build(typeof(SimpleAggregate), aggregate.Id, A<IMemento>.That.Not.IsNull())).MustNotHaveHappened();
    }
  }
}