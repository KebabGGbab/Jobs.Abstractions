using Jobs.Abstractions.Resources;

namespace Jobs.Abstractions
{
    internal sealed class DenyAddHandler : AddHandler
    {
        public DenyAddHandler(List<Job> jobs, bool disposable, List<Job>? queue = null) : base(jobs, disposable, queue)
        {
        }

        public override void Add(Job job, bool isProcessing)
        {
            VerifyAndThrow(job, isProcessing);

            if (isProcessing)
            {
                throw new InvalidOperationException(Strings.AddDenyStrategyTryAdd);
            }

            _jobs.Add(job);
        }
    }
}
