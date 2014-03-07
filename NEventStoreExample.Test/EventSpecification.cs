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
            Caught = null;
            Repository = new InMemoryEventRepository(Given().ToList(), new AggregateFactory());
            var handler = OnHandler();

            try
            {
                handler.Handle(When());
                var expected = Expect().ToList();
                var published = Repository.Events;
                CompareEvents(expected, published);
            }
            catch (AssertionException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Caught = exception;
                throw;
            }
        }

        protected abstract IEnumerable<IEvent> Given();

        protected abstract TCommand When();

        protected abstract ICommandHandler<TCommand> OnHandler();

        protected abstract IEnumerable<IEvent> Expect();

        private static void CompareEvents(ICollection<IEvent> expected, ICollection<IEvent> published)
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