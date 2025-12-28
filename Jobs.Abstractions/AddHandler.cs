using Jobs.Abstractions.Resources;

namespace Jobs.Abstractions
{
    public abstract class AddHandler
    {
        public static readonly AddHandler Deferred = new DeferredAddHandler();
        public static readonly AddHandler Deny = new DenyAddHandler();
        public static readonly AddHandler Force = new ForceAddHandler();

        public abstract bool Add<T>(IList<T> jobs, bool disposable, T job, bool isProcessing, IList<T>? queue) where T : Job;

        protected static void VerifyAndThrow<T>(IList<T> jobs, T job, bool isProcessing, bool disposable) where T : Job
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
