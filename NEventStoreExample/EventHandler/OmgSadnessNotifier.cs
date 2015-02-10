using System;
using NEventStoreExample.Event;
using NEventStoreExample.Infrastructure;

namespace NEventStoreExample.EventHandler
{
  public class OmgSadnessNotifier : IEventHandler<AccountClosedEvent>
  {
    public void Handle(AccountClosedEvent e)
    {
      Console.WriteLine("Dude, we lost a customer... start the layoffs :(");
    }
  }
}