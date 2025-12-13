using Jobs.Abstractions.Resources;

namespace Jobs.Abstractions
{
    internal sealed class DenyAddHandler : AddHandler
    {
        public override bool Add(IList<Job> jobs, bool disposable, Job job, bool isProcessing, IList<Job>? queue)
        {
            VerifyAndThrow(jobs, job, isProcessing, disposable);
            if (isProcessing)
            {
                throw new InvalidOperationException(Strings.AddDenyStrategyTryAdd);
            }

            jobs.Add(job);

            return true;
        }
    }
}
