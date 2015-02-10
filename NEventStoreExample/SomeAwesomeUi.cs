using System;
using CommonDomain.Persistence;
using MemBus;
using NEventStoreExample.Command;

namespace NEventStoreExample
{
  public class SomeAwesomeUi
  {
    private readonly IBus bus;
    private readonly IRepository repository;

    public SomeAwesomeUi(IBus bus)
    {
      this.bus = bus;
      this.repository = this.repository;
    }

    public Guid CreateNewAccount()
    {
      Guid accountID = Guid.NewGuid();
      this.CreateNewAccount(accountID, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
      return accountID;
    }

    public void CreateNewAccount(Guid accountId, string name, string twitter)
    {
      var createCommand = new CreateAccountCommand(accountId, name, twitter);
      this.bus.Publish(createCommand);
    }

    public void CloseAccount(Guid accountId)
    {
      var closeCommand = new CloseAccountCommand(accountId);
      this.bus.Publish(closeCommand);
    }
  }
}