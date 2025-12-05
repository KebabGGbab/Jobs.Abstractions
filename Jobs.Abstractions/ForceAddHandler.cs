namespace Jobs.Abstractions
{
    internal sealed class ForceAddHandler : AddHandler
    {
        public ForceAddHandler(List<Job> jobs, bool disposable, List<Job>? queue) : base(jobs, disposable, queue)
        {
        }

        public override void Add(Job job, bool isProcessing)
        {
            VerifyAndThrow(job, isProcessing);

            _jobs.Add(job);
        }
    }
}
