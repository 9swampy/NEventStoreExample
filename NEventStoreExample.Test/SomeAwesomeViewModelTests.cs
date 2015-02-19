namespace NEventStoreExample.Test
{
  using System;
  using FakeItEasy;
  using MemBus;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Command;

  [TestClass]
  public class SomeAwesomeViewModelTests
  {
    private static IBus bus;
    private static ISomeAwesomeViewModel sut;

    [TestMethod]
    public void ShouldPublishCreateCommandOnTheBus()
    {
      sut.CreateNewAccount();

      A.CallTo(() => bus.Publish(A<CreateAccountCommand>.Ignored)).MustHaveHappened();
    }

    [TestMethod]
    public void ShouldPublishCreateSpecificCommandOnTheBus()
    {
      sut.AccountID = Guid.NewGuid();
      sut.Name = Guid.NewGuid().ToString();
      sut.Twitter = Guid.NewGuid().ToString();

      sut.CreateNewAccount();

      A.CallTo(() => bus.Publish(A<CreateAccountCommand>.Ignored)).WhenArgumentsMatch((args) =>
                                                                                               ((CreateAccountCommand)args[0]).Id == sut.AccountID &&
                                                                                               ((CreateAccountCommand)args[0]).Name == sut.Name &&
                                                                                               ((CreateAccountCommand)args[0]).Twitter == sut.Twitter).MustHaveHappened();
    }

    [TestMethod]
    public void ShouldPublishCloseCommandOnTheBus()
    {
      sut.AccountID = Guid.NewGuid();
      sut.CloseAccount();

      A.CallTo(() => bus.Publish(A<CloseAccountCommand>.Ignored)).MustHaveHappened();
    }

    [TestInitialize]
    public void TestInitialise()
    {
      bus = A.Fake<IBus>();
      sut = new SomeAwesomeViewModel(bus);
    }
  }
}