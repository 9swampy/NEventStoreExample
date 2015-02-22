namespace NEventStoreExample.Event
{
  using System;

  public class AccountCredited : DomainEvent
  {
    public AccountCredited(Guid id, int version, decimal amount) : base(id, version)
    {
      this.Amount = amount;
    }

    public decimal Amount { get; set; }
  }
}