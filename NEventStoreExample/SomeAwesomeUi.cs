namespace NEventStoreExample
{
  using System;
  using MemBus;

  public class SomeAwesomeUi : ISomeAwesomeUi
  {
    private readonly ISomeAwesomeViewModel someAwesomeViewModel;

    public SomeAwesomeUi(IBus bus)
    {
      this.someAwesomeViewModel = new SomeAwesomeViewModel(bus);
    }

    public Guid CreateNewAccount()
    {
      Guid accountID = Guid.NewGuid();
      this.CreateNewAccount(accountID, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
      return accountID;
    }

    public void CreateNewAccount(Guid accountId, string name, string twitter)
    {
      this.someAwesomeViewModel.AccountID = accountId;
      this.someAwesomeViewModel.Name = name;
      this.someAwesomeViewModel.Twitter = twitter;
      this.someAwesomeViewModel.CreateNewAccount();
    }

    public void CloseAccount(Guid accountId)
    {
      this.someAwesomeViewModel.AccountID = accountId;
      this.someAwesomeViewModel.CloseAccount();
    }
  }
}