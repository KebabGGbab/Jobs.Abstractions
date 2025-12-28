namespace Jobs.Abstractions
{
    internal sealed class DeferredAddHandler : AddHandler
    {
        public override bool Add<T>(IList<T> jobs, bool disposable, T job, bool isProcessing, IList<T>? queue)
        {
            ArgumentNullException.ThrowIfNull(queue, nameof(queue));

            VerifyAndThrow(jobs, job, isProcessing, disposable);

            if (isProcessing)
            {
                queue.Add(job);

                return false;
            }
            else
            {
                jobs.Add(job);

                return true;
            }
        }
    }
}
