namespace NEventStoreExample
{
  using System;

  public interface ISomeAwesomeUi
  {
    void CloseAccount(Guid accountId);

    Guid CreateNewAccount();

    void CreateNewAccount(Guid accountId, string name, string twitter);
  }
}