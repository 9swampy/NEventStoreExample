﻿namespace NEventStoreExample.ReadModel
{
  public class AccountDto
  {
    public AccountDto(NEventStoreExample.Domain.Event.AccountCreatedEvent e)
    {
      this.ID = e.AggregateID;
      this.IsActive = e.IsActive;
      this.Name = e.Name;
      this.Twitter = e.Twitter;
      this.Version = e.OriginalVersion;
    }

    public int Version { get; set; }

    public System.Guid ID { get; set; }

    public string Twitter { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }
  }
}