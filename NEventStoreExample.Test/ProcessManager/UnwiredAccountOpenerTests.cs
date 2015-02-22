namespace NEventStoreExample.Test.ProcessManager
{
  using System;
  using System.Linq;
  using FakeItEasy;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Domain.Command;
  using NEventStoreExample.Domain.Event;
  using NEventStoreExample.Domain.ProcessManager;
  using NEventStoreExample.Infrastructure.Bus;

  [TestClass]
  public class UnwiredAccountOpenerTests
  {
    private AccountCreatedEvent accountCreatedEvent;
    private AccountOpener sut;
    private IBus bus;

    [TestInitialize]
    public void TestInitialise()
    {
      this.bus = A.Fake<IBus>();
      this.accountCreatedEvent = new AccountCreatedEvent(Guid.NewGuid(), 0, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false);
      this.sut = new AccountOpener(this.accountCreatedEvent, this.bus);
    }

    [TestMethod]
    public void ShouldInitialiseAnAccountOpener()
    {
      this.sut.Should().NotBeNull();
    }

    [TestMethod]
    public void ShouldPublishValidateAccountCommand()
    {
      this.sut.Execute(0);

      A.CallTo(() => this.bus.Send<ValidateAccountCommand>(A<ValidateAccountCommand>.Ignored)).MustHaveHappened();
    }
  }
}