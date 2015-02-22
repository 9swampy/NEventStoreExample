namespace NEventStoreExample.ProcessManager
{
  using System.Threading.Tasks;
  using NEventStoreExample.Event;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;

  public class AccountOpenerFactory : IEventHandler<AccountCreatedEvent>
  {
    private readonly IBus bus;

    public AccountOpenerFactory(IBus bus)
    {
      this.bus = bus;
    }

    public void Handle(AccountCreatedEvent e)
    {
      AccountOpener accountOpener = new AccountOpener(e, this.bus);
      Task.FromResult(accountOpener.Execute(5));
    }
  }
}