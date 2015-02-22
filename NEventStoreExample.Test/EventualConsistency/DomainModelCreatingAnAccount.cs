namespace NEventStoreExample.Test.EventualConsistency
{
  using System;
  using System.Linq;
  using CommonDomain.Core;
  using CommonDomain.Persistence.EventStore;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStore;
  using NEventStoreExample;
  using NEventStoreExample.CommandHandler;
  using NEventStoreExample.Event;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;

  [TestClass]
  public class DomainModelCreatingAnAccount
  {
    private static ISomeAwesomeUi client;
    private static IBus bus;
    private static Guid accountId;
    private static IStoreEvents store;

    [ClassInitialize]
    public static void ClassInitialise(TestContext context)
    {
      bus = new InProcessBus(DispatchStrategy.Synchronous);

      client = new SomeAwesomeUi(bus);

      store = Wireup.Init().UsingInMemoryPersistence().Build();
      var repository = new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector());
      var handler = new CreateAccountCommandHandler(repository);
      bus.Subscribe(handler);
      accountId = client.CreateNewAccount();
    }
    
    [TestMethod]
    public void ShouldRaiseCreateAccountEventByTheDomainAccountCtorBeingCalledByTheCommandHandler()
    {
      store.OpenStream(accountId, 0, int.MaxValue).CommittedEvents.Count.Should().Be(1);
    }

    [TestMethod]
    public void ShouldStoredAnAccountCreatedEventInTheEventMessageBody()
    {
      store.OpenStream(accountId, 0, int.MaxValue).CommittedEvents.First().Body.Should().BeOfType<AccountCreatedEvent>();
    }

    [TestMethod]
    public void ShouldStoreNothingInTheEventMessageHeaders()
    {
      store.OpenStream(accountId, 0, int.MaxValue).CommittedEvents.First().Headers.Count.Should().Be(0);
    }
  }
}
