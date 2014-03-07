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
    public class when_creating_an_account : EventSpecification<CreateAccountCommand>
    {
        private readonly Guid accountId = Guid.NewGuid();

        protected override IEnumerable<IEvent> Given()
        {
            yield break;
        }

        protected override CreateAccountCommand When()
        {
            return new CreateAccountCommand(accountId, "Luiz Damim", "@luizdamim");
        }

        protected override ICommandHandler<CreateAccountCommand> OnHandler()
        {
            return new CreateAccountCommandHandler(Repository);
        }

        protected override IEnumerable<IEvent> Expect()
        {
            yield return new AccountCreatedEvent(accountId, "Luiz Damim", "@luizdamim", true);
        }
    }
}