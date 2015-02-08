namespace NEventStoreExample.Test
{
  using System;
  using CommonDomain.Core;
  using CommonDomain.Persistence.EventStore;
  using MemBus;
  using MemBus.Configurators;
  using MemBus.Subscribing;
  using NEventStore;
  using NEventStore.Dispatcher;
  using NEventStoreExample.CommandHandler;
  using NEventStoreExample.EventHandler;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Model;
  using Shouldly;
  using Xunit;

  public class EndToEndTest
  {
    private readonly SomeAwesomeUi _client;
    private readonly IBus _bus;

    // Here, I'm wiring up my MemBus instance and telling it how to resolve my subscribers
    // MemBus also has an awesome way to resolve subscribers from an IoC container. In prod,
    // I'll wire my subscribers into StructureMap and have MemBus resolve them from there.
    // I'm also initializing my awesome test client UI which, if you'll recall from way back at the start
    // simply publishes commands to my MemBus instance (and, again, it could be whatever)
    public EndToEndTest()
    {
      _bus = BusSetup.StartWith<Conservative>()
          .Apply<FlexibleSubscribeAdapter>(a =>
          {
            a.ByInterface(typeof(IEventHandler<>));
            a.ByInterface(typeof(ICommandHandler<>));
          })
          .Construct();

      _client = new SomeAwesomeUi(_bus);
    }

    [Fact]
    public void CanPublishCreateAccountCommand()
    {
      Should.NotThrow(() => _client.CreateNewAccount());
    }

    [Fact]
    public void CanReceiveCreateAccountCommand()
    {
      var store = Wireup.Init().UsingInMemoryPersistence().Build();
      var handler = new CreateAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));

      _bus.Subscribe(handler);

      Should.NotThrow(() => _client.CreateNewAccount());
    }

    [Fact]
    public void CreateAccountEventIsStored()
    {
      var store = Wireup.Init().UsingInMemoryPersistence().Build();

      var repository = new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector());
      var handler = new CreateAccountCommandHandler(repository);
      var eventHandler = new AccountDenormalizer();

      _bus.Subscribe(handler);
      _bus.Subscribe(eventHandler);
      var accountId = _client.CreateNewAccount();

      store.OpenStream(accountId, 0, int.MaxValue).CommittedEvents.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void CanLoadAccountFromEventStore()
    {
      var store = Wireup.Init().UsingInMemoryPersistence().Build();
      var repository = new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector());
      var handler = new CreateAccountCommandHandler(repository);

      _bus.Subscribe(handler);

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      _client.CreateNewAccount(accountID, name, twitter);

      Account account = repository.GetById<Account>(accountID);

      account.ShouldNotBe(null);
      account.Name.ShouldBe(name);
      account.Twitter.ShouldBe(twitter);
    }

    [Fact]
    public void CreateAccountEventIsPublishedToBus()
    {
      var store = Wireup.Init().UsingInMemoryPersistence()
                      .UsingSynchronousDispatchScheduler()
                              .DispatchTo(new DelegateMessageDispatcher(c => DelegateDispatcher.DispatchCommit(_bus, c)))
                      .Build();

      var handler = new CreateAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var denormalizer = new AccountDenormalizer();

      _bus.Subscribe(handler);
      _bus.Subscribe(denormalizer);

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      _client.CreateNewAccount(accountID, name, twitter);

      denormalizer.AccountName.ShouldBe(name);
    }

    [Fact]
    public void DeactivingAccountDoesntRetriggerInitialCreate()
    {
      var store = Wireup.Init().UsingInMemoryPersistence()
                      .UsingSynchronousDispatchScheduler()
                              .DispatchTo(new DelegateMessageDispatcher(c => DelegateDispatcher.DispatchCommit(_bus, c)))
                      .Build();

      var createHandler = new CreateAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var deactivateHandler = new CloseAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var denormalizer = new AccountDenormalizer();

      _bus.Subscribe(createHandler);
      _bus.Subscribe(deactivateHandler);
      _bus.Subscribe(denormalizer);

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      _client.CreateNewAccount(accountID, name, twitter);
      _client.CloseAccount(accountID);

      denormalizer.AccountName.ShouldBe(name);
      denormalizer.IsActive.ShouldBe(false);
      store.OpenStream(accountID, 0, int.MaxValue).CommittedEvents.Count.ShouldBe(2);
    }


    //For fun, run this with the Debugger (eg, if using TDD.NET then right click on this method and select Test With -> Debugger.
    //Put break points in various spots of the code above and see what happens.
    [Fact]
    public void TyingItTogether()
    {
      var store = Wireup.Init().UsingInMemoryPersistence()
                      .UsingSynchronousDispatchScheduler()
                              .DispatchTo(new DelegateMessageDispatcher(c => DelegateDispatcher.DispatchCommit(_bus, c)))
                      .Build();

      var createHandler = new CreateAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var deactivateHandler = new CloseAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var denormalizer = new AccountDenormalizer();

      _bus.Subscribe(createHandler);
      _bus.Subscribe(deactivateHandler);

      _bus.Subscribe(denormalizer);
      _bus.Subscribe(new KaChingNotifier());
      _bus.Subscribe(new OmgSadnessNotifier());

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      _client.CreateNewAccount(accountID, name, twitter);
      _client.CloseAccount(accountID);

      denormalizer.AccountName.ShouldBe(name);
      denormalizer.IsActive.ShouldBe(false);
      store.OpenStream(accountID, 0, int.MaxValue).CommittedEvents.Count.ShouldBe(2);
    }
  }
}