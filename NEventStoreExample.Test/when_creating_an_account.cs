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

    protected override IEnumerable<IDomainEvent> Given()
    {
      yield break;
    }

    protected override CreateAccountCommand When()
    {
      return new CreateAccountCommand(this.accountId, "Luiz Damim", "@luizdamim");
    }

    protected override ICommandHandler<CreateAccountCommand> OnHandler()
    {
      return new CreateAccountCommandHandler(this.Repository);
    }

    protected override IEnumerable<IDomainEvent> Expect()
    {
      yield return new AccountCreatedEvent(this.accountId, 0, "Luiz Damim", "@luizdamim", true);
    }
  }
}