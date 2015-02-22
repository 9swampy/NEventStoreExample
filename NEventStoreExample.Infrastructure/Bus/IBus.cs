namespace NEventStoreExample.Infrastructure.Bus
{
  using System;

  public interface IBus : IDisposable, IEventPublisher, ICommandSender, ISubscriber
  {
  }
}