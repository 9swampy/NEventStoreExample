namespace NEventStoreExample
{
  using System;
  using System.Collections.Generic;
  using NEventStoreExample.Domain.Command;
  using NEventStoreExample.Domain.Event;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;
  using NEventStoreExample.ReadModel;

  public class SomeAwesomeViewModel : IEventHandler<AccountCreatedEvent>, IEventHandler<AccountClosedEvent>, ISomeAwesomeViewModel
  {
    private readonly IBus bus;

    private IDictionary<Guid, AccountDto> accountDictionary;

    public SomeAwesomeViewModel(IBus bus)
    {
      this.bus = bus;
    }

    public SomeAwesomeViewModel()
    {
      this.AccountID = Guid.NewGuid();
      this.Name = Guid.NewGuid().ToString();
      this.Twitter = Guid.NewGuid().ToString();
    }

    public Guid AccountID { get; set; }

    public string Name { get; set; }

    public string Twitter { get; set; }

    public IDictionary<Guid, AccountDto> AccountDictionary
    {
      get
      {
        if (this.accountDictionary == null)
        {
          this.accountDictionary = new Dictionary<Guid, AccountDto>();
        }
        return this.accountDictionary;
      }
    }

    public string Output { get; private set; }

    public void CreateNewAccount()
    {
      var createCommand = new CreateAccountCommand(this.AccountID, this.Name, this.Twitter);
      this.bus.Send(createCommand);
    }

    public void CloseAccount()
    {
      AccountDto accountDto;
      int originalVersion;
      if (this.AccountDictionary.TryGetValue(this.AccountID, out accountDto))
      {
        originalVersion = accountDto.Version;
      }
      else
      {
        originalVersion = -1;
      }
      var closeCommand = new CloseAccountCommand(this.AccountID, originalVersion);
      this.bus.Send(closeCommand);
    }

    public void Handle(AccountClosedEvent e)
    {
      AccountDto accountDto;
      if (this.AccountDictionary.TryGetValue(e.AggregateID, out accountDto))
      {
        accountDto.IsActive = false;
      }
    }

    public void Handle(AccountCreatedEvent e)
    {
      this.AccountDictionary.Add(e.AggregateID, new AccountDto(e));
    }
  }
}