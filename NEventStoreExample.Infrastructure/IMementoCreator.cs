namespace NEventStoreExample.Infrastructure
{
  using CommonDomain;

  public interface IMementoCreator
  {
    IMemento CreateMemento();
  }
}