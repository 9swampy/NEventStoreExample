using NEventStoreExample.Event;

namespace NEventStoreExample.Infrastructure
{
  // Normally this class would do something awesome like update Raven
  // There's no reason for this to be a single denormalizer
  // However, there's no reason for this to be multiple denormalizers. Design decision!
  // Our use-case in production is that our denormalizers will update our flattened models in RavenDB
  // although, honestly, it could be SQL Server, Mongo, Raik, whatever.
  public class AccountDenormalizer : IEventHandler<AccountCreatedEvent>, IEventHandler<AccountClosedEvent>
  {
    public string AccountName { get; private set; }

    public bool IsActive { get; private set; }

    public void Handle(AccountCreatedEvent e)
    {
      this.AccountName = e.Name;
    }

    public void Handle(AccountClosedEvent e)
    {
      this.IsActive = false;
    }
  }
}