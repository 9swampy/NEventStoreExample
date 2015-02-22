namespace NEventStoreExample.Test
{
  using System;
  using System.Linq;
  using FakeItEasy;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Domain.Event;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;

  [TestClass]
  public class InProcessBusTests
  {
    [TestMethod]
    public void SubscriptionShouldNotBeNull()
    {
      InProcessBus sut = new InProcessBus();
      IEventHandler<AccountCreatedEvent> fakeHandler = A.Fake<IEventHandler<AccountCreatedEvent>>();
      IDisposable subscription = sut.Subscribe(fakeHandler);
      subscription.Should().NotBeNull();
    }

    [TestMethod]
    public void SubscribeDisposalShouldRemoveSubcription()
    {
      InProcessBus sut = new InProcessBus();
      IEventHandler<AccountCreatedEvent> fakeHandler = A.Fake<IEventHandler<AccountCreatedEvent>>();
      IDisposable subscription = sut.Subscribe(fakeHandler);
      subscription.Dispose();

      sut.Publish(new AccountCreatedEvent(Guid.NewGuid(), 0, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false));

      A.CallTo(() => fakeHandler.Handle(A<AccountCreatedEvent>.Ignored)).MustNotHaveHappened();
    }

    [TestMethod]
    public void SubscribedHandlerShouldHandle()
    {
      InProcessBus sut = new InProcessBus();
      IEventHandler<AccountCreatedEvent> fakeHandler = A.Fake<IEventHandler<AccountCreatedEvent>>();
      IDisposable subscription = sut.Subscribe(fakeHandler);
      
      sut.Publish(new AccountCreatedEvent(Guid.NewGuid(), 0, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false));

      A.CallTo(() => fakeHandler.Handle(A<AccountCreatedEvent>.Ignored)).MustHaveHappened();
    }
  }
}