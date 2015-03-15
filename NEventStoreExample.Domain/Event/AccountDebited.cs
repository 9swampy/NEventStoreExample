namespace NEventStoreExample.Domain.Event
{
  using System;
  using NEventStoreExample.Infrastructure;

  public class AccountDebited : DomainEvent
  {
    public AccountDebited(Guid id, int version, decimal amount) : base(id, version)
    {
      this.Amount = amount;
    }

    public decimal Amount { get; set; }
  }
}