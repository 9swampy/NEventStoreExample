namespace NEventStoreExample
{
  using System;
  using System.Collections.Generic;

  public interface ISomeAwesomeViewModel
  {
    IDictionary<Guid, AccountDto> AccountDictionary { get; }

    string Output { get; }

    Guid AccountID { get; set; }

    string Name { get; set; }

    string Twitter { get; set; }

    void CreateNewAccount();

    void CloseAccount();
  }
}