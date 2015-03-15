namespace NEventStoreExample.Domain.ProcessManager
{
  using System.Threading.Tasks;
  using NEventStoreExample.Domain.Event;
  using NEventStoreExample.Infrastructure;
  using NEventStoreExample.Infrastructure.Bus;

  public class AlarmClockService : IEventHandler<AlarmCreatedEvent>, IAlarmClockService
  {
    private readonly IBus bus;
    
    public AlarmClockService(IBus bus)
    {
      this.bus = bus;
      bus.Subscribe(this);
    }

    public void Handle(AlarmCreatedEvent e)
    {
      Task.Run(async () =>
      {
        await Task.Delay(e.Seconds * 1000);

        this.bus.Publish(new AlarmRaisedEvent(e.AggregateID, e.OriginalVersion + 1, e.CorrelationID, e.CausationID));
      });
    }
  }
}