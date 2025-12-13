namespace Jobs.Abstractions
{
    internal sealed class DeferredAddHandler : AddHandler
    {
        public override bool Add(IList<Job> jobs, bool disposable, Job job, bool isProcessing, IList<Job>? queue)
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
