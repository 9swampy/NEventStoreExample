namespace NEventStoreExample.Domain.Command
{
  using System;

  // Commands to do things are sent to your domain
  // For a great discussion on validation with commands, check out http://ingebrigtsen.info/2012/07/28/cqrs-in-asp-net-mvc-with-bifrost/
  public class CreateAccountCommand : Command
  {
    public CreateAccountCommand(Guid accountId, string name, string twitter) : base(accountId, 0)
    {
      this.Name = name;
      this.Twitter = twitter;
    }

    public string Name { get; private set; }

    public string Twitter { get; private set; }
  }
}