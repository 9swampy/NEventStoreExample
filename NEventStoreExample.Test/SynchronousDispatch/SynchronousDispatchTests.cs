namespace NEventStoreExample.Test.SynchronousDispatch
{
  using System;
  using CommonDomain.Core;
  using CommonDomain.Persistence.EventStore;
  using MemBus;
  using MemBus.Configurators;
  using MemBus.Subscribing;
  using NEventStore;
  using NEventStore.Dispatcher;
  using NEventStoreExample;
  using NEventStoreExample.CommandHandler;
  using NEventStoreExample.EventHandler;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Model;
  using Shouldly;
  using Xunit;

  public class SynchronousDispatchTests
  {
    private readonly SomeAwesomeUi client;
    private readonly IBus bus;

    // Here, I'm wiring up my MemBus instance and telling it how to resolve my subscribers
    // MemBus also has an awesome way to resolve subscribers from an IoC container. In prod,
    // I'll wire my subscribers into StructureMap and have MemBus resolve them from there.
    // I'm also initializing my awesome test client UI which, if you'll recall from way back at the start
    // simply publishes commands to my MemBus instance (and, again, it could be whatever)
    public SynchronousDispatchTests()
    {
      this.bus = BusSetup.StartWith<Conservative>()
                         .Apply<FlexibleSubscribeAdapter>(a =>
                         {
                           a.ByInterface(typeof(IEventHandler<>));
                           a.ByInterface(typeof(ICommandHandler<>));
                         })
                         .Construct();

      this.client = new SomeAwesomeUi(this.bus);
    }

    [Fact]
    public void CanPublishCreateAccountCommand()
    {
      Should.NotThrow(() => this.client.CreateNewAccount());
    }

    [Fact]
    public void CanReceiveCreateAccountCommand()
    {
      var store = Wireup.Init().UsingInMemoryPersistence().Build();
      var handler = new CreateAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));

      this.bus.Subscribe(handler);

      Should.NotThrow(() => this.client.CreateNewAccount());
    }

    [Fact]
    public void CreateAccountEventIsStored()
    {
      var store = Wireup.Init().UsingInMemoryPersistence().Build();

      var repository = new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector());
      var handler = new CreateAccountCommandHandler(repository);
      var eventHandler = new AccountDenormalizer();

      this.bus.Subscribe(handler);
      this.bus.Subscribe(eventHandler);
      var accountId = this.client.CreateNewAccount();

      store.OpenStream(accountId, 0, int.MaxValue).CommittedEvents.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void CanLoadAccountFromEventStore()
    {
      var store = Wireup.Init().UsingInMemoryPersistence().Build();
      var repository = new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector());
      var handler = new CreateAccountCommandHandler(repository);

      this.bus.Subscribe(handler);

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      this.client.CreateNewAccount(accountID, name, twitter);

      Account account = repository.GetById<Account>(accountID);

      account.ShouldNotBe(null);
      account.Name.ShouldBe(name);
      account.Twitter.ShouldBe(twitter);
    }

    [Fact]
    public void CreateAccountEventIsPublishedToBus()
    {
      var store = Wireup.Init()
                        .UsingInMemoryPersistence()
                        .UsingSynchronousDispatchScheduler()
                        .DispatchTo(new DelegateMessageDispatcher(c => DelegateDispatcher.DispatchCommit(this.bus, c)))
                        .Build();

      var handler = new CreateAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var denormalizer = new AccountDenormalizer();

      this.bus.Subscribe(handler);
      this.bus.Subscribe(denormalizer);

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      this.client.CreateNewAccount(accountID, name, twitter);

      denormalizer.AccountName.ShouldBe(name);
    }

    [Fact]
    public void DeactivingAccountDoesntRetriggerInitialCreate()
    {
      var store = Wireup.Init()
                        .UsingInMemoryPersistence()
                        .UsingSynchronousDispatchScheduler()
                        .DispatchTo(new DelegateMessageDispatcher(c => DelegateDispatcher.DispatchCommit(this.bus, c)))
                        .Build();

      var createHandler = new CreateAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var deactivateHandler = new CloseAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var denormalizer = new AccountDenormalizer();

      this.bus.Subscribe(createHandler);
      this.bus.Subscribe(deactivateHandler);
      this.bus.Subscribe(denormalizer);

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      this.client.CreateNewAccount(accountID, name, twitter);
      this.client.CloseAccount(accountID);

      denormalizer.AccountName.ShouldBe(name);
      denormalizer.IsActive.ShouldBe(false);
      store.OpenStream(accountID, 0, int.MaxValue).CommittedEvents.Count.ShouldBe(2);
    }

    //For fun, run this with the Debugger (eg, if using TDD.NET then right click on this method and select Test With -> Debugger.
    //Put break points in various spots of the code above and see what happens.
    [Fact]
    public void TyingItTogether()
    {
      var store = Wireup.Init()
                        .UsingInMemoryPersistence()
                        .UsingSynchronousDispatchScheduler()
                        .DispatchTo(new DelegateMessageDispatcher(c => DelegateDispatcher.DispatchCommit(this.bus, c)))
                        .Build();

      var createHandler = new CreateAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var deactivateHandler = new CloseAccountCommandHandler(new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector()));
      var denormalizer = new AccountDenormalizer();

      this.bus.Subscribe(createHandler);
      this.bus.Subscribe(deactivateHandler);

      this.bus.Subscribe(denormalizer);
      this.bus.Subscribe(new KaChingNotifier());
      this.bus.Subscribe(new OmgSadnessNotifier());

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      this.client.CreateNewAccount(accountID, name, twitter);
      this.client.CloseAccount(accountID);

      denormalizer.AccountName.ShouldBe(name);
      denormalizer.IsActive.ShouldBe(false);
      store.OpenStream(accountID, 0, int.MaxValue).CommittedEvents.Count.ShouldBe(2);
    }
  }
}