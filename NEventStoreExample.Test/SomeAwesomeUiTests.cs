namespace NEventStoreExample.Test
{
  using System;
  using FakeItEasy;
  using MemBus;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Command;

  [TestClass]
  public class SomeAwesomeUiTests
  {
    private static IBus bus;
    private static ISomeAwesomeUi sut;

    [TestMethod]
    public void ShouldPublishCreateCommandOnTheBus()
    {
      sut.CreateNewAccount();

      A.CallTo(() => bus.Publish(A<CreateAccountCommand>.Ignored)).MustHaveHappened();
    }

    [TestMethod]
    public void ShouldPublishCreateSpecificCommandOnTheBus()
    {
      Guid accountID = Guid.NewGuid();
      string name = Guid.NewGuid().ToString();
      string twitter = Guid.NewGuid().ToString();

      sut.CreateNewAccount(accountID, name, twitter);

      A.CallTo(() => bus.Publish(A<CreateAccountCommand>.Ignored)).WhenArgumentsMatch((args) =>
                                                                                               ((CreateAccountCommand)args[0]).Id == accountID &&
                                                                                               ((CreateAccountCommand)args[0]).Name == name &&
                                                                                               ((CreateAccountCommand)args[0]).Twitter == twitter).MustHaveHappened();
    }

    [TestMethod]
    public void ShouldPublishCloseCommandOnTheBus()
    {
      Guid accountID = Guid.NewGuid();
      sut.CloseAccount(accountID);

      A.CallTo(() => bus.Publish(A<CloseAccountCommand>.Ignored)).MustHaveHappened();
    }

    [TestInitialize]
    public void TestInitialise()
    {
      bus = A.Fake<IBus>();
      sut = new SomeAwesomeUi(bus);
    }
  }
}