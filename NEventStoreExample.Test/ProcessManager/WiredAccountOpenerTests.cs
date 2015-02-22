namespace NEventStoreExample.Test.ProcessManager
{
  using System;
  using System.Threading.Tasks;
  using FakeItEasy;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Command;
  using NEventStoreExample.Event;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;
  using NEventStoreExample.ProcessManager;

  [TestClass]
  public class WiredAccountOpenerTests
  {
    private static AccountCreatedEvent accountCreatedEvent;
    private static AccountOpener sut;
    private static IBus bus;

    [ClassInitialize]
    public static void ClassInitialise(TestContext ctx)
    {

    }

    [TestInitialize]
    public void TestInitialise()
    {
      bus = new InProcessBus();
      accountCreatedEvent = new AccountCreatedEvent(Guid.NewGuid(), 0, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false);
      sut = new AccountOpener(accountCreatedEvent, bus);
      bus.Subscribe(sut);
      AlarmClockService alarmClockService = new AlarmClockService(bus);
    }

    [TestMethod]
    public async Task ShouldReceiveAnAlarmRaisedEvent()
    {
      ICommandHandler<ValidateAccountCommand> fakeCommandHandler = A.Fake<ICommandHandler<ValidateAccountCommand>>();
      bus.Subscribe(fakeCommandHandler);

      await sut.Execute(0);

      sut.Response.Should().Be(AccountOpenerResponse.TimedOut);
    }

    [TestMethod]
    public async Task ShouldValidateTheAccount()
    {
      ICommandHandler<ValidateAccountCommand> fakeCommandHandler = A.Fake<ICommandHandler<ValidateAccountCommand>>();
      A.CallTo(() => fakeCommandHandler.Handle(A<ValidateAccountCommand>.Ignored)).Invokes(args =>
      {
        ValidateAccountCommand command = args.Arguments[0] as ValidateAccountCommand;
        bus.Publish(new AccountValidatedEvent(command.AggregateID, command.OriginalVersion, command.CorrelationID, command.AggregateID));
      });
      bus.Subscribe(fakeCommandHandler);

      await sut.Execute(3);

      sut.Response.Should().Be(AccountOpenerResponse.Validated);
    }

    [TestMethod]
    public async Task ShouldInvalidateTheAccount()
    {
      ICommandHandler<ValidateAccountCommand> fakeCommandHandler = A.Fake<ICommandHandler<ValidateAccountCommand>>();
      A.CallTo(() => fakeCommandHandler.Handle(A<ValidateAccountCommand>.Ignored)).Invokes(args =>
      {
        ValidateAccountCommand command = args.Arguments[0] as ValidateAccountCommand;
        bus.Publish(new AccountInvalidatedEvent(command.AggregateID, command.OriginalVersion, command.CorrelationID, command.AggregateID));
      });
      bus.Subscribe(fakeCommandHandler);

      await sut.Execute(3);

      sut.Response.Should().Be(AccountOpenerResponse.Invalidated);
    }

    [TestMethod]
    public async Task ShouldCloseTheAccount()
    {
      ICommandHandler<ValidateAccountCommand> fakeCommandHandler = A.Fake<ICommandHandler<ValidateAccountCommand>>();
      A.CallTo(() => fakeCommandHandler.Handle(A<ValidateAccountCommand>.Ignored)).Invokes(args =>
      {
        ValidateAccountCommand command = args.Arguments[0] as ValidateAccountCommand;
        bus.Publish(new AccountClosedEvent(command.AggregateID, command.OriginalVersion, command.CorrelationID, command.AggregateID));
      });
      bus.Subscribe(fakeCommandHandler);

      await sut.Execute(3);

      sut.Response.Should().Be(AccountOpenerResponse.Closed);
    }
  }
}