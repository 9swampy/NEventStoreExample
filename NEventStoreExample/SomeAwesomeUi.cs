using System;
using CommonDomain.Persistence;
using MemBus;
using NEventStoreExample.Command;
using NEventStoreExample.Model;

namespace NEventStoreExample
{
  public class SomeAwesomeUi
  {
    private readonly IBus bus;
    private IRepository repository;

    public SomeAwesomeUi(IBus bus)
    {
      this.bus = bus;
      this.repository = repository;
    }

    public Guid CreateNewAccount()
    {
      Guid accountID = Guid.NewGuid();
      CreateNewAccount(accountID, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
      return accountID;
    }

    public void CreateNewAccount(Guid accountId, string name, string twitter)
    {
      var createCommand = new CreateAccountCommand(accountId, name, twitter);
      bus.Publish(createCommand);
    }

    public void CloseAccount(Guid accountId)
    {
      var closeCommand = new CloseAccountCommand(accountId);
      bus.Publish(closeCommand);
    }
  }
}