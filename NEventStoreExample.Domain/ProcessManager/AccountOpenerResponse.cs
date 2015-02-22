namespace NEventStoreExample.Domain.ProcessManager
{
  public enum AccountOpenerResponse
  {
    Pending,
    TimedOut,
    Validated,
    Invalidated,
    Closed
  }
}