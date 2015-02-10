using System;

namespace NEventStoreExample.Model
{
  public class AccountId
  {
    public AccountId(Guid value)
    {
      this.Value = value;
    }
        
    public Guid Value { get; private set; }
  }
}