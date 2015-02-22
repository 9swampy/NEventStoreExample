namespace NEventStoreExample.Infrastructure
{
  public interface ICommandHandler<in TCommand>
  {
    void Handle(TCommand command);
  }
}