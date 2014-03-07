using System;
using System.Collections.Generic;
using NEventStoreExample.Command;
using NEventStoreExample.CommandHandler;
using NEventStoreExample.Event;
using NEventStoreExample.Infrastructure;
using NUnit.Framework;

namespace NEventStoreExample.Test
{
    [TestFixture]
    public class when_closing_an_account : EventSpecification<CloseAccountCommand>
    {
        private readonly Guid accountId = Guid.NewGuid();

        protected override IEnumerable<IEvent> Given()
        {
            yield return new AccountCreatedEvent(accountId, "Luiz Damim", "@luizdamim", true);
        }

        protected override CloseAccountCommand When()
        {
            return new CloseAccountCommand(accountId);
        }

        protected override ICommandHandler<CloseAccountCommand> OnHandler()
        {
            return new CloseAccountCommandHandler(Repository);
        }

        protected override IEnumerable<IEvent> Expect()
        {
            yield return new AccountClosedEvent();
        }
    }
}