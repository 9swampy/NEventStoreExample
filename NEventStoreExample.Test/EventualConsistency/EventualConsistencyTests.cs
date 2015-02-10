namespace NEventStoreExample.Test.EventualConsistency
{
  using System;
  using System.Threading.Tasks;
  using CommonDomain.Core;
  using CommonDomain.Persistence.EventStore;
  using MemBus;
  using MemBus.Configurators;
  using MemBus.Subscribing;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStore;
  using NEventStore.Client;
  using NEventStoreExample;
  using NEventStoreExample.CommandHandler;
  using NEventStoreExample.EventHandler;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Model;
  using Shouldly;

  [TestClass]
  public class EventualConsistencyTests
  {
    private static SomeAwesomeUi client;
    private static IBus bus;

    [ClassInitialize]
    public static void ClassInitialise(TestContext context)
    {
      bus = BusSetup.StartWith<Conservative>()
                    .Apply<FlexibleSubscribeAdapter>(a =>
                    {
                      a.ByInterface(typeof(IEventHandler<>));
                      a.ByInterface(typeof(ICommandHandler<>));
                    })
                    .Construct();

      client = new SomeAwesomeUi(bus);
    }

    [TestMethod]
    public void CanPublishCreateAccountCommand()
    {
      Should.NotThrow(() => client.CreateNewAccount());
    }

    [TestMethod]
    public void CanReceiveCreateAccountCommand()
    {
      var store = Wireup.Init().UsingInMemoryPersistence().Build();
      var handler = new CreateAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));

      bus.Subscribe(handler);

      Should.NotThrow(() => client.CreateNewAccount());
    }

    [TestMethod]
    public void CreateAccountEventIsStored()
    {
      var store = Wireup.Init().UsingInMemoryPersistence().Build();

      var repository = new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector());
      var handler = new CreateAccountCommandHandler(repository);
      var eventHandler = new AccountDenormalizer();

      bus.Subscribe(handler);
      bus.Subscribe(eventHandler);
      var accountId = client.CreateNewAccount();

      store.OpenStream(accountId, 0, int.MaxValue).CommittedEvents.Count.ShouldBeGreaterThan(0);
    }

    [TestMethod]
    public void CanLoadAccountFromEventStore()
    {
      var store = Wireup.Init().UsingInMemoryPersistence().Build();
      var repository = new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector());
      var handler = new CreateAccountCommandHandler(repository);

      bus.Subscribe(handler);

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      client.CreateNewAccount(accountID, name, twitter);

      Account account = repository.GetById<Account>(accountID);

      account.ShouldNotBe(null);
      account.Name.ShouldBe(name);
      account.Twitter.ShouldBe(twitter);
    }

    [TestMethod]
    public async Task CreateAccountEventIsPublishedToBus()
    {
      var store = Wireup.Init()
                        .UsingInMemoryPersistence()
                        .Build();

      var repository = new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector());
      var handler = new CreateAccountCommandHandler(repository);

      var massTransitDispatcher = new MassTransitDispatcher(bus);
      var pollingClient = new PollingClient(store.Advanced);
      var commitObserver = pollingClient.ObserveFrom(null);

      var denormalizer = new AccountDenormalizer();

      bus.Subscribe(handler);
      bus.Subscribe(denormalizer);

      using (PollingHook pollingHook = new PollingHook(commitObserver))
      {
        using (var subscription = commitObserver.Subscribe(massTransitDispatcher))
        {
          commitObserver.Start();

          Guid accountID = Guid.NewGuid();
          string name = Guid.NewGuid().ToString();
          string twitter = Guid.NewGuid().ToString();

          client.CreateNewAccount(accountID, name, twitter);

          DateTime timeoutEnd = DateTime.Now.AddSeconds(10);
          while (denormalizer.AccountName != name && DateTime.Now < timeoutEnd)
          {
            await Task.Delay(100);
          }

          denormalizer.AccountName.ShouldBe(name);
        }
      }
    }

    [TestMethod]
    public async Task DeactivingAccountDoesntRetriggerInitialCreate()
    {
      var store = Wireup.Init()
                        .UsingInMemoryPersistence()
                        .Build();

      var createHandler = new CreateAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var deactivateHandler = new CloseAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var denormalizer = new AccountDenormalizer();

      bus.Subscribe(createHandler);
      bus.Subscribe(deactivateHandler);
      bus.Subscribe(denormalizer);

      var massTransitDispatcher = new MassTransitDispatcher(bus);
      var pollingClient = new PollingClient(store.Advanced);
      var commitObserver = pollingClient.ObserveFrom(null);

      using (PollingHook pollingHook = new PollingHook(commitObserver))
      {
        using (var subscription = commitObserver.Subscribe(massTransitDispatcher))
        {
          commitObserver.Start();

          Guid accountID = Guid.NewGuid();
          string name = Guid.NewGuid().ToString();
          string twitter = Guid.NewGuid().ToString();

          client.CreateNewAccount(accountID, name, twitter);
          client.CloseAccount(accountID);

          DateTime timeoutEnd = DateTime.Now.AddSeconds(10);
          while ((denormalizer.AccountName != name ||
                  denormalizer.IsActive) &&
                 DateTime.Now < timeoutEnd)
          {
            await Task.Delay(100);
          }

          denormalizer.AccountName.ShouldBe(name);
          denormalizer.IsActive.ShouldBe(false);
          store.OpenStream(accountID, 0, int.MaxValue).CommittedEvents.Count.ShouldBe(2);
        }
      }
    }

    [TestMethod]
    public async Task TyingItTogether()
    {
      var store = Wireup.Init()
                        .UsingInMemoryPersistence()
                        .Build();

      var createHandler = new CreateAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var deactivateHandler = new CloseAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var denormalizer = new AccountDenormalizer();

      bus.Subscribe(createHandler);
      bus.Subscribe(deactivateHandler);

      bus.Subscribe(denormalizer);
      bus.Subscribe(new KaChingNotifier());
      bus.Subscribe(new OmgSadnessNotifier());

      var massTransitDispatcher = new MassTransitDispatcher(bus);
      var pollingClient = new PollingClient(store.Advanced);
      var commitObserver = pollingClient.ObserveFrom(null);

      using (PollingHook pollingHook = new PollingHook(commitObserver))
      {
        using (var subscription = commitObserver.Subscribe(massTransitDispatcher))
        {
          commitObserver.Start();

          Guid accountID = Guid.NewGuid();
          string name = Guid.NewGuid().ToString();
          string twitter = Guid.NewGuid().ToString();

          client.CreateNewAccount(accountID, name, twitter);
          client.CloseAccount(accountID);

          DateTime timeoutEnd = DateTime.Now.AddSeconds(10);
          while ((denormalizer.AccountName != name ||
                  denormalizer.IsActive) &&
                 DateTime.Now < timeoutEnd)
          {
            await Task.Delay(100);
          }

          denormalizer.AccountName.ShouldBe(name);
          denormalizer.IsActive.ShouldBe(false);
          store.OpenStream(accountID, 0, int.MaxValue).CommittedEvents.Count.ShouldBe(2);
        }
      }
    }
  }
}