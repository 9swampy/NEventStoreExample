namespace NEventStoreExample.Infrastructure.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using CommonDomain;
  using CommonDomain.Core;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using NEventStoreExample.Infrastructure.Tests.SimpleDomain;
  using FluentAssertions;
  using FakeItEasy;

  [TestClass]
  public class AggregateFactoryTests
  {
    private class AggregateWithInsufficientCtor : AggregateBase
    {
      private AggregateWithInsufficientCtor(bool aParameterToRemoveDefaultCtorless)
      { }
    }

    [TestMethod]
    public void ShouldCreateANewEmptyAggregate()
    {
      AggregateFactory aggregateFactory = new AggregateFactory();
      SimpleAggregate aggregate = new SimpleAggregate();
      IAggregate rebuiltAggregate = aggregateFactory.Build(aggregate.GetType(), aggregate.Id, null);

      rebuiltAggregate.ShouldBeEquivalentTo(aggregate);
    }

    [TestMethod]
    public void ShouldRehydrateTheAggregateFromSnapshot()
    {
      AggregateFactory aggregateFactory = new AggregateFactory();
      SimpleAggregate aggregate = new SimpleAggregate(Guid.NewGuid(), DateTime.Now);

      IAggregate rebuiltAggregate = aggregateFactory.Build(aggregate.GetType(), aggregate.Id, ((IMementoCreator)aggregate).CreateMemento());

      rebuiltAggregate.ShouldBeEquivalentTo(aggregate);
    }

    [TestMethod]
    public void ShouldThrowCtorMissingError()
    {
      AggregateFactory aggregateFactory = new AggregateFactory();

      Action act = () => aggregateFactory.Build(typeof(AggregateWithInsufficientCtor), Guid.NewGuid(), null);

      act.ShouldThrow<InvalidOperationException>().Where(ex => ex.Message == string.Format("Aggregate {0} cannot be created: no parameterless non-public constructor has been provided", typeof(AggregateWithInsufficientCtor).Name));
    }

    [TestMethod]
    public void ShouldThrowRehydrateCtorMissingError()
    {
      AggregateFactory aggregateFactory = new AggregateFactory();
      Type type = typeof(AggregateWithInsufficientCtor);

      Action act = () => aggregateFactory.Build(type, Guid.NewGuid(), A.Fake<IMemento>());

      act.ShouldThrow<InvalidOperationException>().Where(ex => ex.Message == string.Format("Aggregate {0} cannot be created: no non-public constructor that accepts IMemento has been provided", type.Name));
    }
  }
}
