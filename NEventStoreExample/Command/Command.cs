namespace NEventStoreExample.Command
{
  using System;
  using NEventStoreExample.Infrastructure;

  public abstract class Command : ICommand
  {
    private readonly int originalVersion;
    private readonly Guid aggregateID;

    public Command(Guid aggregateID, int originalVersion)
    {
      this.aggregateID = aggregateID;
      this.originalVersion = originalVersion;
    }

    public int OriginalVersion
    {
      get
      {
        return this.originalVersion;
      }
    }

    public Guid AggregateID
    {
      get
      {
        return this.aggregateID;
      }
    }
  }
}