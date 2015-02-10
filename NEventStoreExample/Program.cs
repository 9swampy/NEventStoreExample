using System;
using CommonDomain.Core;
using CommonDomain.Persistence.EventStore;
using MemBus;
using MemBus.Configurators;
using MemBus.Subscribing;
using NEventStore;
using NEventStore.Dispatcher;
using NEventStoreExample.CommandHandler;
using NEventStoreExample.EventHandler;
using NEventStoreExample.Infrastructure;

namespace NEventStoreExample
{
  public class Program
  {
    private static readonly Guid AggregateId = Guid.NewGuid();

    private static IStoreEvents store;

    public static void Main(string[] args)
    {
      var bus = BusSetup.StartWith<Conservative>()
                        .Apply<FlexibleSubscribeAdapter>(a =>
                        {
                          a.ByInterface(typeof(IEventHandler<>));
                          a.ByInterface(typeof(ICommandHandler<>));
                        })
                        .Construct();

      var someAwesomeUi = new SomeAwesomeUi(bus);

      using (store = WireupEventStore(bus))
      {
        var repository = new EventStoreRepository(store, new AggregateFactory(), new ConflictDetector());

        var handler = new CreateAccountCommandHandler(repository);
        var handler2 = new CloseAccountCommandHandler(repository);
        bus.Subscribe(handler);
        bus.Subscribe(handler2);
        bus.Subscribe(new KaChingNotifier());
        bus.Subscribe(new OmgSadnessNotifier());

        someAwesomeUi.CreateNewAccount(AggregateId, "Luiz", "@luizdamim");
        someAwesomeUi.CloseAccount(AggregateId);
      }

      Console.ReadLine();
    }

    private static IStoreEvents WireupEventStore(IBus bus)
    {
      return Wireup.Init()
      ////.LogToOutputWindow()
      ////.LogToConsoleWindow()
                   .UsingInMemoryPersistence()
                   .UsingJsonSerialization()
                   .Compress()
                   .UsingSynchronousDispatchScheduler()
                   .DispatchTo(new DelegateMessageDispatcher(c => DelegateDispatcher.DispatchCommit(bus, c)))
                   .Build();
    }
  }
}