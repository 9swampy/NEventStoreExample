namespace NEventStoreExample.Infrastructure.EventualConsistency
{
  using NEventStore;
  using NEventStore.Client;

  internal class PollingHook : PipelineHookBase
  {
    private readonly IObserveCommits commitsObserver;

    public PollingHook(IObserveCommits commitsObserver)
    {
      this.commitsObserver = commitsObserver;
    }

    public override void PostCommit(ICommit committed)
    {
      base.PostCommit(committed);
      this.commitsObserver.PollNow();
    }
  }
}