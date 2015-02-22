namespace NEventStoreExample.Domain.Command
{
  using System;
  using NEventStoreExample.Infrastructure;

  public class ValidateAccountCommand : Command
  {
    public ValidateAccountCommand(Guid accountId, int originalVersion, Guid correlationID) : base(accountId, originalVersion, correlationID)
    {
    }
  }
}