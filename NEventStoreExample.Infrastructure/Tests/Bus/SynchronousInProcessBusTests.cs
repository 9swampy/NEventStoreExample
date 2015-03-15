namespace NEventStoreExample.Infrastructure.Tests.Bus
{
  using System;
  using FakeItEasy;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Infrastructure.Bus;
  using NEventStoreExample.Infrastructure.Tests.SimpleDomain;

  [TestClass]
  public class SynchronousInProcessBusTests
  {
    private static InProcessBus sut;
    private static ICommandHandler<PeekSimpleAggregate> peekHandler;
    private static IEventHandler<SimpleAggregateCreated> createdHandler;

    [ClassInitialize]
    public static void ClassInitialise(TestContext context)
    {
      sut = new InProcessBus();
      sut.Subscribe(A.Fake<ICommandHandler<PokeSimpleAggregate>>());
      sut.Subscribe(A.Fake<ICommandHandler<PokeSimpleAggregate>>());
      sut.Subscribe(A.Fake<IEventHandler<SimpleAggregateCreated>>());
      createdHandler = A.Fake<IEventHandler<SimpleAggregateCreated>>();
      sut.Subscribe(createdHandler);
      peekHandler = A.Fake<ICommandHandler<PeekSimpleAggregate>>();
      sut.Subscribe(peekHandler);
    }

    [TestMethod]
    public void UnhandledSendShouldThrow()
    {
      Action act = () => sut.Send(new UpdateSimpleAggregate(Guid.NewGuid(), 0, DateTime.Now));

      act.ShouldThrow<InvalidOperationException>().Where(e => e.Message == string.Format("No handler registered for {0}", typeof(UpdateSimpleAggregate).Name));
    }

    [TestMethod]
    public void MultiHandledSendShouldThrow()
    {
      Action act = () => sut.Send(new PokeSimpleAggregate(Guid.NewGuid(), 0, DateTime.Now));

      act.ShouldThrow<InvalidOperationException>().Where(e => e.Message == string.Format("Cannot send {0} to more than one handler", typeof(PokeSimpleAggregate).Name));
    }

    [TestMethod]
    public void HandledSendShouldBeSent()
    {
      sut.Send(new PeekSimpleAggregate(Guid.NewGuid(), 0));

      A.CallTo(() => peekHandler.Handle(A<PeekSimpleAggregate>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
    }

    [TestMethod]
    public void HandledPublishShouldBeSent()
    {
      sut.Publish(new SimpleAggregateCreated(Guid.NewGuid(), DateTime.Now));

      A.CallTo(() => createdHandler.Handle(A<SimpleAggregateCreated>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      sut.Dispose();
    }
  }
}