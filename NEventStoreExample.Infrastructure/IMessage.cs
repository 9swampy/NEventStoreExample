namespace NEventStoreExample.Infrastructure
{
  using System;

  public interface IMessage
  {
    Guid AggregateID { get; }

    int OriginalVersion { get; }
  }
}