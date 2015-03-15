namespace NEventStoreExample.Infrastructure.Tests
{
  using FakeItEasy;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStore;

  [TestClass]
  public class InMemoryDomainRepositoryTests
  {
    [TestMethod]
    public void ShouldStoreEventStore()
    {
      IStoreEvents eventStore = A.Fake<IStoreEvents>();
      InMemoryDomainRepository sut = new InMemoryDomainRepository(eventStore);
      sut.EventStore.Should().Be(eventStore);
    }

    [TestMethod]
    public void ShouldPopulateDefaultStoreEventStore()
    {
      InMemoryDomainRepository sut = new InMemoryDomainRepository();
      sut.EventStore.Should().NotBeNull();
    }
  }
}
