namespace NEventStoreExample.Infrastructure.Tests.Bus
{
  using System;
  using FakeItEasy;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Infrastructure.Bus;
  using NEventStoreExample.Infrastructure.Tests.SimpleDomain;

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
}