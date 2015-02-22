namespace NEventStoreExample.Infrastructure
{
  using System;
  
  public abstract class Command : ICommand
  {
    private readonly int originalVersion;
    private readonly Guid aggregateID;
    private readonly Guid correlationID;

    public Command(Guid aggregateID, int originalVersion) : this(aggregateID, originalVersion, Guid.NewGuid())
    {
    }

    public Command(Guid aggregateID, int originalVersion, Guid correlationID)
    {
      this.aggregateID = aggregateID;
      this.originalVersion = originalVersion;
      this.correlationID = correlationID;
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

    public Guid CorrelationID
    {
      get
      {
        return this.correlationID;
      }
    }
  }
}