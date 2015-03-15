namespace NEventStoreExample.Infrastructure.Tests.Bus
{
  using System;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;
  using NEventStoreExample.Infrastructure.Tests.SimpleDomain;

  [TestClass]
  public class DelegateAdjusterTests
  {
    [TestMethod]
    public void AdjustedHandlerShouldHandleMessage()
    {
      bool isProcessed = false;
      Action<SimpleAggregateUpdated> handler = (t) =>
      {
        isProcessed = true;
      };
      Action<IMessage> adjustedHandler = DelegateAdjuster.CastArgument<IMessage, SimpleAggregateUpdated>(x => handler(x));
      Action act = () => adjustedHandler(new SimpleAggregateUpdated(Guid.NewGuid(), 0, DateTime.Now));

      act();

      isProcessed.Should().BeTrue();
    }

    [TestMethod]
    public void UnadjustedHandlerShouldHandleMessage()
    {
      bool isProcessed = false;
      Action<SimpleAggregateUpdated> handler = (t) =>
      {
        isProcessed = true;
      };
      Action<SimpleAggregateUpdated> adjustedHandler = DelegateAdjuster.CastArgument<SimpleAggregateUpdated, SimpleAggregateUpdated>(x => handler(x));
      Action act = () => adjustedHandler(new SimpleAggregateUpdated(Guid.NewGuid(), 0, DateTime.Now));

      act();

      isProcessed.Should().BeTrue();
    }
  }
}
