using System;
using CommonDomain.Persistence;
using NEventStoreExample.Command;
using NEventStoreExample.Infrastructure;
using NEventStoreExample.Model;

namespace NEventStoreExample.CommandHandler
{
    // This may _look_ like a normal "load this by id and then mutate state and then save it" repository
    // However, it's actually loading the entire event stream for that object and re-applying the events to
    // it. Don't worry, though, it's not re-publishing events to the bus.. it's just raising events
    // internally to the object. Your domain object, at the end, will be the culmination of all of those
    // applied events. This would be much simpler in F# of we thought about domain state as a left fold
    // of immutable events causing state change.
    //
    // One neat thing about EventStore and, by extension, CommonDomain, is that you can load versions of your
    // object. Check out the overloads on _repository.GetById some time.
    public class CloseAccountCommandHandler : ICommandHandler<CloseAccountCommand>
    {
        private readonly IRepository repository;

        public CloseAccountCommandHandler(IRepository repository)
        {
            this.repository = repository;
        }

        public void Handle(CloseAccountCommand command)
        {
            var account = repository.GetById<Account>(command.AccountId);
            account.Close();
            repository.Save(account, Guid.NewGuid());
        }
    }
}