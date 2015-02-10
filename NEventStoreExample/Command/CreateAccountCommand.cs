using System;
using NEventStoreExample.Infrastructure;

namespace NEventStoreExample.Command
{
  // Commands to do things are sent to your domain
  // For a great discussion on validation with commands, check out http://ingebrigtsen.info/2012/07/28/cqrs-in-asp-net-mvc-with-bifrost/
  public class CreateAccountCommand : ICommand
  {
    public CreateAccountCommand(Guid accountId, string name, string twitter)
    {
      this.Id = accountId;
      this.Name = name;
      this.Twitter = twitter;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Twitter { get; private set; }
  }
}