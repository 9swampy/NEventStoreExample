namespace NEventStoreExample
{
  using System;
  using MemBus;
  using NEventStoreExample.Command;

  public class SomeAwesomeUi : NEventStoreExample.ISomeAwesomeUi
  {
    private readonly IBus bus;

    public SomeAwesomeUi(IBus bus)
    {
      this.bus = bus;
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