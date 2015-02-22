namespace NEventStoreExample.Infrastructure
{
  using System;

  public interface ICommand : IMessage
  {
    Guid CorrelationID { get; }
  }
}