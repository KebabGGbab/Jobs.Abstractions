using Jobs.Abstractions.Resources;

namespace Jobs.Abstractions
{
    internal sealed class DenyAddHandler : AddHandler
    {
        public override bool Add<T>(IList<T> jobs, bool disposable, T job, bool isProcessing, IList<T>? queue)
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
