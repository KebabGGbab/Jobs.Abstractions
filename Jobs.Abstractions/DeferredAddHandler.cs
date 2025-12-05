namespace Jobs.Abstractions
{
    internal sealed class DeferredAddHandler : AddHandler
    {
        public DeferredAddHandler(List<Job> jobs, bool disposable, List<Job>? queue) : base(jobs, disposable, queue)
        {
            ArgumentNullException.ThrowIfNull(queue, nameof(queue));
        }

        public override void Add(Job job, bool isProcessing)
        {
            VerifyAndThrow(job, isProcessing);

            if (isProcessing)
            {
                _queue!.Add(job);
            }
            else
            {
                _jobs.Add(job);
            }
        }
    }
}
