namespace NEventStoreExample.Infrastructure
{
  using System;
  using CommonDomain.Persistence;
  using NEventStore;

  public interface IEventStoreRepository : IRepository, IDisposable
  {
    IStoreEvents EventStore { get; }
  }
}