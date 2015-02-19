using System;
using System.Collections.Generic;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using NEventStoreExample.Infrastructure;
using NUnit.Framework;

namespace NEventStoreExample.Test
{
  public abstract class EventSpecification<TCommand> where TCommand : class, ICommand
  {
    protected Exception Caught { get; private set; }

    protected InMemoryEventRepository Repository { get; private set; }

    [Test]
    public void SetUp()
    {
      this.Caught = null;
      this.Repository = new InMemoryEventRepository(this.Given().ToList(), new AggregateFactory());
      var handler = this.OnHandler();

      try
      {
        handler.Handle(this.When());
        var expected = this.Expect().ToList();
        var published = this.Repository.Events;
        CompareEvents(expected, published);
      }
      catch (AssertionException)
      {
        throw;
      }
      catch (Exception exception)
      {
        this.Caught = exception;
        throw;
      }
    }

    protected abstract IEnumerable<IDomainEvent> Given();

    protected abstract TCommand When();

    protected abstract ICommandHandler<TCommand> OnHandler();

    protected abstract IEnumerable<IDomainEvent> Expect();

    private static void CompareEvents(ICollection<IDomainEvent> expected, ICollection<IDomainEvent> published)
    {
      Assert.That(published.Count, Is.EqualTo(expected.Count), "Different number of expected/published events.");

      var compareObjects = new CompareObjects();

      var eventPairs = expected.Zip(published, (e, p) => new { Expected = e, Produced = p });
      foreach (var events in eventPairs)
      {
        compareObjects.ActualName = events.Produced.GetType().Name;
        compareObjects.ExpectedName = events.Expected.GetType().Name;
        if (!compareObjects.Compare(events.Expected, events.Produced))
        {
          Assert.Fail(compareObjects.DifferencesString);
        }
      }
    }
  }
}