namespace NEventStoreExample.Command
{
  using System;
  using NEventStoreExample.Infrastructure;

  // A command doesn't need to carry state if you don't want it to... Here, we're just telling it the account id to close.
  public class CloseAccountCommand : Command
  {
    public CloseAccountCommand(Guid accountId, int originalVersion) : base(accountId, originalVersion)
    {
      this.AccountId = accountId;
    }

    public Guid AccountId { get; private set; }
  }
}