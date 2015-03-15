namespace NEventStoreExample.Infrastructure
{
  using System;
  using System.Reflection;
  using CommonDomain;
  using CommonDomain.Persistence;

  public class AggregateFactory : IConstructAggregates
  {
    public IAggregate Build(Type type, Guid id, IMemento snapshot)
    {
      if (snapshot == null)
      {
        return BuildFromStart(type);
      }

      ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { snapshot.GetType() }, null);
      if (constructor == null)
      {
        throw new InvalidOperationException(string.Format("Aggregate {0} cannot be created: no non-public constructor that accepts IMemento has been provided", type.Name));
      }
      return constructor.Invoke(new object[] { snapshot }) as IAggregate;
    }

    private static IAggregate BuildFromStart(Type type)
    {
      ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
      if (constructor == null)
      {
        throw new InvalidOperationException(string.Format("Aggregate {0} cannot be created: no parameterless non-public constructor has been provided", type.Name));
      }
      return constructor.Invoke(null) as IAggregate;
    }
  }
}