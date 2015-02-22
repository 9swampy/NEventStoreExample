namespace NEventStoreExample.Test
{
  using System;
  using FakeItEasy;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Domain.Command;
  using NEventStoreExample.Infrastructure.Bus;

  [TestClass]
  public class SomeAwesomeViewModelTests
  {
    private static IBus bus;
    private static ISomeAwesomeViewModel sut;
    
    [TestInitialize]
    public void TestInitialise()
    {
      bus = A.Fake<IBus>();
      sut = new SomeAwesomeViewModel(bus);
    }

    [TestMethod]
    public void ShouldPublishCreateCommandOnTheBus()
    {
      sut.CreateNewAccount();

      A.CallTo(() => bus.Send(A<CreateAccountCommand>.Ignored)).MustHaveHappened();
    }

    [TestMethod]
    public void ShouldPublishCreateSpecificCommandOnTheBus()
    {
      sut.AccountID = Guid.NewGuid();
      sut.Name = Guid.NewGuid().ToString();
      sut.Twitter = Guid.NewGuid().ToString();

      sut.CreateNewAccount();

      A.CallTo(() => bus.Send(A<CreateAccountCommand>.Ignored)).WhenArgumentsMatch((args) =>
                                                                                            ((CreateAccountCommand)args[0]).AggregateID == sut.AccountID &&
                                                                                            ((CreateAccountCommand)args[0]).Name == sut.Name &&
                                                                                            ((CreateAccountCommand)args[0]).Twitter == sut.Twitter).MustHaveHappened();
    }

    [TestMethod]
    public void ShouldPublishCloseCommandOnTheBus()
    {
      sut.AccountID = Guid.NewGuid();
      sut.CloseAccount();

      A.CallTo(() => bus.Send(A<CloseAccountCommand>.Ignored)).MustHaveHappened();
    }
  }
}