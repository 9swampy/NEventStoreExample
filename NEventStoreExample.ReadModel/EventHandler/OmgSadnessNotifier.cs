using System;
using NEventStoreExample.Domain.Event;
using NEventStoreExample.Infrastructure;

namespace NEventStoreExample.ReadModel.EventHandler
{
  public class OmgSadnessNotifier : IEventHandler<AccountClosedEvent>
  {
    public void Handle(AccountClosedEvent e)
    {
      Console.WriteLine("Dude, we lost a customer... start the layoffs :(");
    }
  }
}