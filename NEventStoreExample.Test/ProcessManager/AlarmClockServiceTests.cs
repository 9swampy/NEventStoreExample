namespace NEventStoreExample.Test
{
  using System;
  using FakeItEasy;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Domain.Event;
  using NEventStoreExample.Domain.ProcessManager;
  using NEventStoreExample.Infrastructure.Bus;

  [TestClass]
  public class AlarmClockServiceTests
  {
    private static AlarmClockService sut;
    private static IBus bus;

    [ClassInitialize]
    public static void ClassInitialise(TestContext ctx)
    {
      bus = A.Fake<IBus>();
      sut = new AlarmClockService(bus);
    }

    [TestMethod]
    public void ShouldInitialiseNewAlarmClockService()
    {
      sut.Should().NotBeNull();
    }

    [TestMethod]
    public void ShouldPublishAlarmRaisedEvent()
    {
      Guid id = Guid.NewGuid();
      Guid correlationID = Guid.NewGuid();
      Guid causationID = Guid.NewGuid();
      sut.Handle(new AlarmCreatedEvent(id, 0, correlationID, causationID, 0));
      A.CallTo(() => (bus as IEventPublisher).Publish<AlarmRaisedEvent>(A<AlarmRaisedEvent>.Ignored)).MustHaveHappened();
    }
  }
}