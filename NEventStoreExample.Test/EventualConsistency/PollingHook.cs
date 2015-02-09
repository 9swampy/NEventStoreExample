namespace NEventStoreExample.Test.EventualConsistency
{
  using NEventStore;
  using NEventStore.Client;

  internal class PollingHook : PipelineHookBase
  {
    private readonly IObserveCommits _commitsObserver;

    public PollingHook(IObserveCommits commitsObserver)
    {
      _commitsObserver = commitsObserver;
    }

    public override void PostCommit(ICommit committed)
    {
      base.PostCommit(committed);
      _commitsObserver.PollNow();
    }
  }
}