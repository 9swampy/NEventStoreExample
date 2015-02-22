namespace NEventStoreExample.Test.ProcessManager
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using FakeItEasy;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using FluentAssertions;
  using NEventStoreExample.Command;
  using NEventStoreExample.Event;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;
  using NEventStoreExample.ProcessManager;

  [TestClass]
  public class UnwiredAccountOpenerTests
  {
    private AccountCreatedEvent accountCreatedEvent;
    private AccountOpener sut;
    private IBus bus;

    [TestInitialize]
    public void TestInitialise()
    {
      bus = A.Fake<IBus>();
      accountCreatedEvent = new AccountCreatedEvent(Guid.NewGuid(), 0, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false);
      sut = new AccountOpener(accountCreatedEvent, bus);
    }

    [TestMethod]
    public void ShouldInitialiseAnAccountOpener()
    {
      sut.Should().NotBeNull();
    }

    [TestMethod]
    public void ShouldPublishValidateAccountCommand()
    {
      sut.Execute(0);

      A.CallTo(() => bus.Send<ValidateAccountCommand>(A<ValidateAccountCommand>.Ignored)).MustHaveHappened();
    }
  }
}
