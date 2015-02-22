namespace NEventStoreExample.Test.SynchronousDispatch
{
  using System;
  using CommonDomain.Core;
  using CommonDomain.Persistence.EventStore;
  using FluentAssertions;
  using NEventStore;
  using NEventStore.Dispatcher;
  using NEventStoreExample;
  using NEventStoreExample.Domain.CommandHandler;
  using NEventStoreExample.Domain.Model;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;
  using NEventStoreExample.ReadModel;
  using NEventStoreExample.ReadModel.EventHandler;
  using Xunit;

  public class SynchronousDispatchTests
  {
    private readonly ISomeAwesomeUi client;
    private readonly IBus bus;
    private readonly IStoreEvents store;
    private readonly EventStoreRepository repository;

    // Here, I'm wiring up my MemBus instance and telling it how to resolve my subscribers
    // MemBus also has an awesome way to resolve subscribers from an IoC container. In prod,
    // I'll wire my subscribers into StructureMap and have MemBus resolve them from there.
    // I'm also initializing my awesome test client UI which, if you'll recall from way back at the start
    // simply publishes commands to my MemBus instance (and, again, it could be whatever)
    public SynchronousDispatchTests()
    {
      this.bus = new InProcessBus(DispatchStrategy.Synchronous);

      #pragma warning disable 0618
      this.store = Wireup.Init()
                         .UsingInMemoryPersistence()
                         .UsingSynchronousDispatchScheduler()
                         .DispatchTo(new DelegateMessageDispatcher(c => DelegateDispatcher.DispatchCommit(this.bus, c)))
                         .Build();
      #pragma warning restore 0618

      this.repository = new EventStoreRepository(this.store, new AggregateFactory(), new ConflictDetector());
      var handler = new CreateAccountCommandHandler(this.repository);
      
      this.bus.Subscribe(handler);

      this.client = new SomeAwesomeUi(this.bus);
    }

    [Fact]
    public void CanPublishCreateAccountCommand()
    {
      Action act = () => this.client.CreateNewAccount();
      act.ShouldNotThrow();
    }

    [Fact]
    public void CanReceiveCreateAccountCommand()
    {
      Action act = () => this.client.CreateNewAccount();
      act.ShouldNotThrow();
    }

    [Fact]
    public void CreateAccountEventIsStored()
    {
      var eventHandler = new AccountDenormalizer();
      this.bus.Subscribe(eventHandler);

      var accountId = this.client.CreateNewAccount();

      this.store.OpenStream(accountId, 0, int.MaxValue).CommittedEvents.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CanLoadAccountFromEventStore()
    {
      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      this.client.CreateNewAccount(accountID, name, twitter);

      Account account = this.repository.GetById<Account>(accountID);

      account.Should().NotBeNull();
      account.Name.Should().Be(name);
      account.Twitter.Should().Be(twitter);
    }

    [Fact]
    public void CreateAccountEventIsPublishedToBus()
    {
      var denormalizer = new AccountDenormalizer();

      this.bus.Subscribe(denormalizer);

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      this.client.CreateNewAccount(accountID, name, twitter);

      denormalizer.AccountName.Should().Be(name);
    }

    [Fact]
    public void DeactivingAccountDoesntRetriggerInitialCreate()
    {
      var deactivateHandler = new CloseAccountCommandHandler(new EventStoreRepository(this.store, new AggregateFactory(), new ConflictDetector()));
      var denormalizer = new AccountDenormalizer();

      this.bus.Subscribe(deactivateHandler);
      this.bus.Subscribe(denormalizer);

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      this.client.CreateNewAccount(accountID, name, twitter);
      this.client.CloseAccount(accountID);

      denormalizer.AccountName.Should().Be(name);
      denormalizer.IsActive.Should().Be(false);
      this.store.OpenStream(accountID, 0, int.MaxValue).CommittedEvents.Count.Should().Be(2);
    }

    //For fun, run this with the Debugger (eg, if using TDD.NET then right click on this method and select Test With -> Debugger.
    //Put break points in various spots of the code above and see what happens.
    [Fact]
    public void TyingItTogether()
    {
      var deactivateHandler = new CloseAccountCommandHandler(new EventStoreRepository(this.store, new AggregateFactory(), new ConflictDetector()));
      var denormalizer = new AccountDenormalizer();

      this.bus.Subscribe(deactivateHandler);

      this.bus.Subscribe(denormalizer);
      this.bus.Subscribe(new KaChingNotifier());
      this.bus.Subscribe(new OmgSadnessNotifier());

      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      this.client.CreateNewAccount(accountID, name, twitter);
      this.client.CloseAccount(accountID);

      denormalizer.AccountName.Should().Be(name);
      denormalizer.IsActive.Should().Be(false);
      this.store.OpenStream(accountID, 0, int.MaxValue).CommittedEvents.Count.Should().Be(2);
    }
  }
}