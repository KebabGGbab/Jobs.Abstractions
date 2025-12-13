namespace Jobs.Abstractions
{
    internal sealed class ForceAddHandler : AddHandler
    {
        public override bool Add(IList<Job> jobs, bool disposable, Job job, bool isProcessing, IList<Job>? queue)
        {
            VerifyAndThrow(jobs, job, isProcessing, disposable);

            jobs.Add(job);

            return true;
        }
    }
}
