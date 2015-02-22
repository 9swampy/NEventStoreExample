﻿namespace NEventStoreExample.Domain.ProcessManager
{
  using System;
  using System.Threading.Tasks;
  using NEventStoreExample.Domain.Event;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;

  public class AccountOpener : IEventHandler<AccountInvalidatedEvent>, IEventHandler<AccountValidatedEvent>, IEventHandler<AccountClosedEvent>, IEventHandler<AlarmRaisedEvent>
  {
    private readonly IBus bus;
    private readonly AccountCreatedEvent accountCreatedEvent;
    private readonly Guid correlationID;

    private AccountOpenerResponse response;
    private TaskCompletionSource<object> tcs;

    public AccountOpener(AccountCreatedEvent e, IBus bus)
    {
      this.bus = bus;
      this.accountCreatedEvent = e;
      this.correlationID = Guid.NewGuid();
      this.response = AccountOpenerResponse.Pending;
    }

    public AccountOpenerResponse Response
    {
      get
      {
        return this.response;
      }
      set
      {
        this.response = value;
        if (this.tcs != null)
        {
          this.tcs.SetResult(null);
        }
      }
    }

    public Guid AccountID
    {
      get
      {
        return this.accountCreatedEvent.ID;
      }
    }

    public Guid CorrelationID
    {
      get
      {
        return this.correlationID;
      }
    }

    private bool IsPending
    {
      get
      {
        return this.Response == AccountOpenerResponse.Pending;
      }
    }

    public async Task Execute(int timeoutSeconds)
    {
      this.bus.Send(new NEventStoreExample.Domain.Command.ValidateAccountCommand(this.AccountID, this.accountCreatedEvent.Version, this.CorrelationID));
      this.tcs = new TaskCompletionSource<object>();
      this.bus.Publish(new AlarmCreatedEvent(this.AccountID, this.accountCreatedEvent.Version, this.CorrelationID, this.accountCreatedEvent.ID, timeoutSeconds));
      while (this.IsPending)
      {
        await Task.WhenAny(this.tcs.Task, Task.Delay(30000));
      }
    }

    public void Handle(AccountInvalidatedEvent e)
    {
      if (e.ID == this.AccountID && e.CorrelationID == this.CorrelationID)
      {
        this.Response = AccountOpenerResponse.Invalidated;
      }
    }

    public void Handle(AccountValidatedEvent e)
    {
      if (e.ID == this.AccountID && e.CorrelationID == this.CorrelationID)
      {
        this.Response = AccountOpenerResponse.Validated;
      }
    }

    public void Handle(AccountClosedEvent e)
    {
      if (e.ID == this.AccountID && e.CorrelationID == this.CorrelationID)
      {
        this.Response = AccountOpenerResponse.Closed;
      }
    }

    public void Handle(AlarmRaisedEvent e)
    {
      if (e.ID == this.AccountID && e.CorrelationID == this.CorrelationID)
      {
        this.Response = AccountOpenerResponse.TimedOut;
      }
    }
  }
}