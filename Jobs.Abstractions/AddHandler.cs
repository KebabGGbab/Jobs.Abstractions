using Jobs.Abstractions.Resources;

namespace Jobs.Abstractions
{
    public abstract class AddHandler
    {
        public static readonly AddHandler Deferred = new DeferredAddHandler();
        public static readonly AddHandler Deny = new DenyAddHandler();
        public static readonly AddHandler Force = new ForceAddHandler();

        public abstract bool Add(IList<Job> jobs, bool disposable, Job job, bool isProcessing, IList<Job>? queue);

        protected static void VerifyAndThrow(IList<Job> jobs, Job job, bool isProcessing, bool disposable)
        {
            ArgumentNullException.ThrowIfNull(jobs, nameof(jobs));

            if (disposable && isProcessing)
            {
                throw new InvalidOperationException(Strings.AddJobInDisposableManager);
            }

            ArgumentNullException.ThrowIfNull(job, nameof(job));
        }
    }
}
