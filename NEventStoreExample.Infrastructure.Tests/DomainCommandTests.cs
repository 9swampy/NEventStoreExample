namespace NEventStoreExample.Infrastructure.Tests
{
  using System;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class DomainCommandTests
  {
    private static Guid id;
    private static int version;
    private static Command sut;

    private class TestDomainCommand : Command
    {
      internal TestDomainCommand(Guid id, int version)
        : base(id, version)
      { }
    }

    [ClassInitialize]
    public static void ClassInitialise(TestContext context)
    {
      id = Guid.NewGuid();
      version = new Random().Next();
      sut = new TestDomainCommand(id, version);
    }

    [TestMethod]
    public void ShouldStoreId()
    {
      sut.AggregateID.Should().Be(id);
    }

    [TestMethod]
    public void ShouldStoreVersion()
    {
      sut.OriginalVersion.Should().Be(version);
    }

    [TestMethod]
    public void ShouldGenerateADefaultCorrelationID()
    {
      sut.CorrelationID.Should().NotBeEmpty();
    }
  }
}