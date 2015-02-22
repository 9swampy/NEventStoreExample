namespace NEventStoreExample.Domain.Command
{
  using System;

  public class ValidateAccountCommand : Command
  {
    public ValidateAccountCommand(Guid accountId, int originalVersion, Guid correlationID) : base(accountId, originalVersion, correlationID)
    {
    }
  }
}