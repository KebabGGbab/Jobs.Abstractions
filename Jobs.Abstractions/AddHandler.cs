using Jobs.Abstractions.Resources;

namespace Jobs.Abstractions
{
    internal abstract class AddHandler
    {
        protected readonly List<Job> _jobs;
        protected readonly List<Job>? _queue;
        protected readonly bool _disposable;

        public AddHandler(List<Job> jobs, bool disposable, List<Job>? queue = null)
        {
            ArgumentNullException.ThrowIfNull(jobs, nameof(jobs));

            _jobs = jobs;
            _queue = queue;
            _disposable = disposable;
        }

        public abstract void Add(Job job, bool isProcessing);

        protected void VerifyAndThrow(Job job, bool isProcessing)
        {
            if (_disposable && isProcessing)
            {
                throw new InvalidOperationException(Strings.AddJobInDisposableManager);
            }

            ArgumentNullException.ThrowIfNull(job, nameof(job));
        }
    }
}
