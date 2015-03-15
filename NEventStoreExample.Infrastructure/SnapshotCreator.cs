namespace NEventStoreExample.Infrastructure
{
  using System;
  using CommonDomain;
  using CommonDomain.Core;
  using LiteGuard;
  using NEventStore;

  public class SnapshotCreator<T> where T : AggregateBase, IMementoCreator
  {
    private readonly IEventStoreRepository repo;
    private readonly int delta;

    public SnapshotCreator(IEventStoreRepository repo, int delta)
    {
      Guard.AgainstNullArgument("repo", repo);
      this.repo = repo;
      this.delta = delta;
    }

    /// <summary>
    /// Save new Aggregate Snapshot depending on specific Snapshoting policies.
    /// NOTE: In real context, it should be an external thread save snapshots, without interfere with online process
    /// </summary>
    public Snapshot SaveSnapShot(Guid id)
    {
      Snapshot result = null;
      T aggregate = this.repo.GetById<T>(id);
      if (aggregate.Version % delta == 0)
      {
        IMemento memento = ((IMementoCreator)aggregate).CreateMemento();
        result = new Snapshot(aggregate.Id.ToString(), aggregate.Version, memento);
        this.repo.EventStore.Advanced.AddSnapshot(result);
      }
      return result;
    }
  }
}