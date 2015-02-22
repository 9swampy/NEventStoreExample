namespace NEventStoreExample.Infrastructure.Bus
{
  using System;

  public interface ISubscriber
  {
    IDisposable Subscribe(object subscriber);

    IDisposable Subscribe<M>(Action<M> subscription);
  }
}