namespace NEventStoreExample.Infrastructure.Tests.Bus
{
  using System;
  using FakeItEasy;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Infrastructure.Bus;
  using NEventStoreExample.Infrastructure.Tests.SimpleDomain;
  using FluentAssertions;
  using System.Threading;

  [TestClass]
  public class AsynchronousInProcessBusTests
  {
    private static InProcessBus sut;
    private static ICommandHandler<PeekSimpleAggregate> peekHandler;
    private static IEventHandler<SimpleAggregateCreated> createdHandler;

    [ClassInitialize]
    public static void ClassInitialise(TestContext context)
    {
      sut = new InProcessBus(DispatchStrategy.Asynchronous);
      sut.Subscribe(A.Fake<IEventHandler<SimpleAggregateCreated>>());
      int callCount = 0;
      createdHandler = A.Fake<IEventHandler<SimpleAggregateCreated>>();
      A.CallTo(() => createdHandler.Handle(A<SimpleAggregateCreated>.Ignored)).Invokes(() => callCount++);
      sut.Subscribe(createdHandler);
      peekHandler = A.Fake<ICommandHandler<PeekSimpleAggregate>>();
      A.CallTo(() => peekHandler.Handle(A<PeekSimpleAggregate>.Ignored)).Invokes(() => callCount++);
      sut.Subscribe(peekHandler);

      DateTime timeoutEnd = DateTime.Now.AddSeconds(10);
      while (callCount < 2 && timeoutEnd < DateTime.Now)
      {
        Thread.Sleep(10);
      }
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      sut.Dispose();
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
  }

  [TestClass]
  public class InProcessBusDisposalTests
  {
    [TestMethod]
    public void DisposalShouldThrowAsNotImplementedYet()
    {
      Action act = () =>
      {
        using (InProcessBus sut = new InProcessBus())
        {

        }
      };

      act.ShouldThrow<NotImplementedException>();
    }

    [TestMethod]
    public void DisposalShouldLeaveCommandUnhandled()
    {
      InProcessBus sut = new InProcessBus();
      Action actPost = () => sut.Send(new PeekSimpleAggregate(Guid.NewGuid(), 0));

      Action act = () =>
          {
            ICommandHandler<PeekSimpleAggregate> peekHandler = A.Fake<ICommandHandler<PeekSimpleAggregate>>();
            using (sut.Subscribe(peekHandler))
            {
              actPost.ShouldNotThrow();
            }
          };
      act.ShouldNotThrow();

      actPost.ShouldThrow<InvalidOperationException>().Where(e => e.Message == string.Format("No handler registered for {0}", typeof(PeekSimpleAggregate).Name));
    }
  }

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