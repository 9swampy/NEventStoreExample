using System;
using CommonDomain.Persistence;
using NEventStoreExample.Command;
using NEventStoreExample.Infrastructure;
using NEventStoreExample.Model;

namespace NEventStoreExample.CommandHandler
{
  // This is the handler that will apply commands to my domain. I could choose
  // another round of some sort of non-business rule validation here. I could
  // log stuff. Whatever. There's also no reason that you need one CommandHandler
  // per command. I'm just doing this because I think this is how our real impl will shape out.
  // IRepository comes from CommonDomain and is a facade over EventStore (both by Jonathan Oliver)
  public class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand>
  {
    private readonly IRepository repository;

    public CreateAccountCommandHandler(IRepository repository)
    {
      this.repository = repository;
    }

    public void Handle(CreateAccountCommand command)
    {
      var account = new Account(new AccountId(command.AggregateID), command.Name, command.Twitter);

      this.repository.Save(account, Guid.NewGuid());
    }
  }
}