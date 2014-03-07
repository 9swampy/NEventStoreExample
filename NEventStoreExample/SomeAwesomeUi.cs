using System;
using MemBus;
using NEventStoreExample.Command;

namespace NEventStoreExample
{
    public class SomeAwesomeUi
    {
        private readonly IBus bus;

        public SomeAwesomeUi(IBus bus)
        {
            this.bus = bus;
        }

        public void CreateNewAccount(Guid accountId, string name, string twitter)
        {
            var createCommand = new CreateAccountCommand(accountId, name, twitter);
            bus.Publish(createCommand);
        }

        public void CloseAccount(Guid accountId)
        {
            var closeCommand = new CloseAccountCommand(accountId);
            bus.Publish(closeCommand);
        }
    }
}