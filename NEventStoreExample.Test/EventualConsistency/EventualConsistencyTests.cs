namespace NEventStoreExample.Test.EventualConsistency
{
  using System;
  using System.Threading.Tasks;
  using CommonDomain.Core;
  using CommonDomain.Persistence.EventStore;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStore;
  using NEventStore.Client;
  using NEventStoreExample;
  using NEventStoreExample.Domain.CommandHandler;
  using NEventStoreExample.Domain.Model;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;
  using NEventStoreExample.ReadModel;
  using NEventStoreExample.ReadModel.EventHandler;

  [TestClass]
  public class EventualConsistencyTests
  {
    private static ISomeAwesomeUi client;
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
      
      var createHandler = new CreateAccountCommandHandler(repository);
      var deactivateHandler = new CloseAccountCommandHandler(repository);

      bus.Subscribe(createHandler);
      bus.Subscribe(deactivateHandler);
      
      client = new SomeAwesomeUi(bus);
    }

    [TestMethod]
    public void ShouldHaveInitialisedTheClient()
    {
      client.Should().NotBeNull();
    }

    [TestMethod]
    public void ShouldHaveInitialisedTheBus()
    {
      bus.Should().NotBeNull();
    }

    [TestMethod]
    public void CanPublishCreateAccountCommand()
    {
      Action act = () => client.CreateNewAccount();
      act.ShouldNotThrow();
    }

    [TestMethod]
    public void CanReceiveCreateAccountCommand()
    {
      Action act = () => client.CreateNewAccount();

      act.ShouldNotThrow();
    }

    [TestMethod]
    public void CreateAccountEventIsStored()
    {
      var accountId = client.CreateNewAccount();
      store.OpenStream(accountId, 0, int.MaxValue).CommittedEvents.Count.Should().Be(1);
    }

    [TestMethod]
    public void CreateAccountEventIsStoredAndWhyBotherHavingTheEventHandlerInThisTest()
    {
      var eventHandler = new AccountDenormalizer();

      bus.Subscribe(eventHandler);
      var accountId = client.CreateNewAccount();

      store.OpenStream(accountId, 0, int.MaxValue).CommittedEvents.Count.Should().Be(1);
    }

    [TestMethod]
    public void CanLoadAccountFromEventStore()
    {
      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      client.CreateNewAccount(accountID, name, twitter);

      Account account = repository.GetById<Account>(accountID);

      account.Should().NotBeNull();
      account.Name.Should().Be(name);
      account.Twitter.Should().Be(twitter);
    }

    [TestMethod]
    public async Task CreateAccountEventIsPublishedToBus()
    {
      MassTransitDispatcher massTransitDispatcher = new MassTransitDispatcher(bus);
      PollingClient pollingClient = new PollingClient(store.Advanced, 100);
      IObserveCommits commitObserver = pollingClient.ObserveFrom(null);

      AccountDenormalizer denormalizer = new AccountDenormalizer();

      bus.Subscribe(denormalizer);

      using (PollingHook pollingHook = new PollingHook(commitObserver))
      {
        using (var subscription = commitObserver.Subscribe(massTransitDispatcher))
        {
          commitObserver.PollNow();
          commitObserver.Start();

          Guid accountID = Guid.NewGuid();
          string name = Guid.NewGuid().ToString();
          string twitter = Guid.NewGuid().ToString();

          System.Diagnostics.Debug.Print(string.Format("Creating account {0}", accountID));
          client.CreateNewAccount(accountID, name, twitter);

          DateTime timeoutEnd = DateTime.Now.AddSeconds(10);
          while (denormalizer.AccountName != name && DateTime.Now < timeoutEnd)
          {
            await Task.Delay(100);
          }

          denormalizer.AccountName.Should().Be(name);
        }
      }
    }

    [TestMethod]
    public async Task DeactivingAccountDoesntRetriggerInitialCreate()
    {
      var denormalizer = new AccountDenormalizer();

      bus.Subscribe(denormalizer);

      var massTransitDispatcher = new MassTransitDispatcher(bus);
      var pollingClient = new PollingClient(store.Advanced, 100);
      var commitObserver = pollingClient.ObserveFrom(null);

      using (PollingHook pollingHook = new PollingHook(commitObserver))
      {
        using (var subscription = commitObserver.Subscribe(massTransitDispatcher))
        {
          commitObserver.PollNow();
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

          denormalizer.AccountName.Should().Be(name);
          denormalizer.IsActive.Should().Be(false);
          store.OpenStream(accountID, 0, int.MaxValue).CommittedEvents.Count.Should().Be(2);
        }
      }
    }

    [TestMethod]
    public async Task TyingItTogether()
    {
      var denormalizer = new AccountDenormalizer();

      bus.Subscribe(denormalizer);
      bus.Subscribe(new KaChingNotifier());
      bus.Subscribe(new OmgSadnessNotifier());

      var massTransitDispatcher = new MassTransitDispatcher(bus);
      var pollingClient = new PollingClient(store.Advanced, 100);
      var commitObserver = pollingClient.ObserveFrom(null);

      using (PollingHook pollingHook = new PollingHook(commitObserver))
      {
        using (var subscription = commitObserver.Subscribe(massTransitDispatcher))
        {
          commitObserver.PollNow();
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

          denormalizer.AccountName.Should().Be(name);
          denormalizer.IsActive.Should().Be(false);
          store.OpenStream(accountID, 0, int.MaxValue).CommittedEvents.Count.Should().Be(2);
        }
      }
    }
  }
}