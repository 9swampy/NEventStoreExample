namespace NEventStoreExample.Infrastructure.Tests
{
  using System;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using FluentAssertions;
  using NEventStoreExample.Infrastructure.Tests.SimpleDomain;

  [TestClass]
  public class DomainEventTests
  {
    private static Guid id;
    private static int version;
    private static DomainEvent sut;

    private class TestDomainEvent : DomainEvent
    {
      internal TestDomainEvent(Guid id, int version)
        : base(id, version)
      { }
    }

    [ClassInitialize]
    public static void ClassInitialise(TestContext context)
    {
      id = Guid.NewGuid();
      version = new Random().Next();
      sut = new TestDomainEvent(id, version);
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
  }
}
