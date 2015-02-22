namespace NEventStoreExample.ProcessManager
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