namespace NEventStoreExample.Infrastructure.Bus
{
  public interface IHandleMessage<T>
  {
    void Handle(T message);
  }
}