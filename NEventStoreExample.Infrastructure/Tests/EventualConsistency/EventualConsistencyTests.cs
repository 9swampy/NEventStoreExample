namespace NEventStoreExample.Infrastructure.Tests.EventualConsistency
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using CommonDomain.Core;
  using CommonDomain.Persistence.EventStore;
  using FakeItEasy;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStore;
  using NEventStore.Client;
  using NEventStoreExample.Infrastructure.Bus;
  using NEventStoreExample.Infrastructure.EventualConsistency;
  using NEventStoreExample.Infrastructure.Tests.SimpleDomain;
  using FluentAssertions;

  [TestClass]
  public class EventualConsistencyTests
  {
    private static IBus bus;
    private static EventStoreRepository repository;
    private static IStoreEvents store;

    [ClassInitialize]
    public static void ClassInitialise(TestContext context)
    {
      bus = new InProcessBus(DispatchStrategy.Asynchronous);

      store = Wireup.Init()
                    .UsingInMemoryPersistence()
                    .Build();
      repository = new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector());
    }

    [TestMethod]
    public void CreateAccountEventIsPublishedToBus()
    {
      using (MassTransitDispatcher massTransitDispatcher = new MassTransitDispatcher(bus))
      {
        PollingClient pollingClient = new PollingClient(store.Advanced, 100);
        IObserveCommits commitObserver = pollingClient.ObserveFrom(null);

        IEventHandler<SimpleAggregateCreated> denormalizer = A.Fake<IEventHandler<SimpleAggregateCreated>>();
        AutoResetEvent are = new AutoResetEvent(false);
        A.CallTo(() => denormalizer.Handle(A<SimpleAggregateCreated>.Ignored)).Invokes(() => are.Set());

        bus.Subscribe(denormalizer);

        using (PollingHook pollingHook = new PollingHook(commitObserver))
        {
          using (var subscription = commitObserver.Subscribe(massTransitDispatcher))
          {
            commitObserver.PollNow();
            commitObserver.Start();

            Guid aggregateID = Guid.NewGuid();

            SimpleAggregate aggregate = new SimpleAggregate(aggregateID, DateTime.Now);
            repository.Save(aggregate, Guid.NewGuid(), (o) => { });

            are.WaitOne(10000).Should().BeTrue("event should be dispatched and recieved within timeout");
          }
        }
      }
    }
  }
}