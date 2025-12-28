namespace Jobs.Abstractions
{
    internal sealed class ForceAddHandler : AddHandler
    {
        public override bool Add<T>(IList<T> jobs, bool disposable, T job, bool isProcessing, IList<T>? queue)
        {
            VerifyAndThrow(jobs, job, isProcessing, disposable);

            jobs.Add(job);

            return true;
        }
    }
}
