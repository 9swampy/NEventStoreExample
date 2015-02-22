namespace NEventStoreExample.Test
{
  using System;
  using System.Collections.Generic;
  using NEventStoreExample.Domain.Command;
  using NEventStoreExample.Domain.CommandHandler;
  using NEventStoreExample.Domain.Event;
  using NEventStoreExample.Infrastructure;
  using NUnit.Framework;

  [TestFixture]
  public class when_closing_an_account : EventSpecification<CloseAccountCommand>
  {
    private readonly Guid accountId = Guid.NewGuid();

    protected override IEnumerable<IDomainEvent> Given()
    {
      yield return new AccountCreatedEvent(this.accountId, -1, "Luiz Damim", "@luizdamim", true);
    }

    protected override CloseAccountCommand When()
    {
      return new CloseAccountCommand(this.accountId, 0);
    }

    protected override ICommandHandler<CloseAccountCommand> OnHandler()
    {
      return new CloseAccountCommandHandler(this.Repository);
    }

    protected override IEnumerable<IDomainEvent> Expect()
    {
      yield return new AccountClosedEvent(this.accountId, 1, Guid.NewGuid(), Guid.Empty);
    }
  }
}