namespace NEventStoreExample.Infrastructure.Tests.Bus
{
  using System;
  using System.Threading;
  using FakeItEasy;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Infrastructure.Bus;
  using NEventStoreExample.Infrastructure.Tests.SimpleDomain;

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
        Thread.Sleep(1000);
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
}