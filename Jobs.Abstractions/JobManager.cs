using Jobs.Abstractions.Resources;

namespace Jobs.Abstractions
{
    public abstract class JobManager
    {
        private readonly List<Job> _jobs = [];
        private readonly List<Job>? _queue;
        private readonly bool _isDisposable;
        private readonly bool _clean;
        private readonly AddHandler _addHandler;

        private int _progress;

        public int Progress 
        { 
            get => _progress; 
            protected set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Jobs.Count);

                _progress = value;

                if (value == Jobs.Count)
                {
                    Complete();
                }

            }
        }
        public virtual bool IsProcessing { get; protected set; }
        public IReadOnlyList<Job> Jobs => _jobs;

        public event EventHandler? Completed;
        
        public JobManager(JobContext context) : this(context, new JobManagerOptions()) { }

        public JobManager(JobContext context, JobManagerOptions options)
        { 
            _isDisposable = options.Disposable;
            _clean = options.Clean;

            if (!_isDisposable)
            {
                _queue = [];
            }

            _addHandler = AddHandlerResolver.Resolve(_jobs, _queue, _isDisposable, options.AddStrategy);
        }

        public JobManager(JobContext context, Job job) : this(context, [job]) { }

        public JobManager(JobContext context, IEnumerable<Job> jobs , JobManagerOptions? options = null) : this(context, options ?? new JobManagerOptions())
        {
            ArgumentNullException.ThrowIfNull(jobs, nameof(jobs));
            _jobs.Capacity = jobs.Count();
            _jobs.AddRange(jobs);
        }

        public abstract Task RunAsync();

        public void AddJob(Job job)
        {
            _addHandler.Add(job, IsProcessing);
        }

        public abstract void Cancel();

        private void Complete()
        {
            Refresh();
            Completed?.Invoke(this, EventArgs.Empty);
        }
        
        private void Refresh()
        {
            if (!_isDisposable && _clean)
            {
                _jobs.Clear();
                _queue!.ForEach(_jobs.Add);
                _queue.Clear();
            }
        }

        protected virtual void OnTaskCompleted(object? sender, EventArgs e)
        {
            Interlocked.Increment(ref _progress);
        }

        private static class AddHandlerResolver
        {
            public static AddHandler Resolve(List<Job> jobs, List<Job>? queue, bool isDisposable, AddStrategy strategy)
            {
                return strategy switch
                {
                    AddStrategy.Deferred => new DeferredAddHandler(jobs, isDisposable, queue),
                    AddStrategy.Deny => new DenyAddHandler(jobs, isDisposable, queue),
                    AddStrategy.Force => new ForceAddHandler(jobs, isDisposable, queue),
                    _ =>  throw new InvalidOperationException(Strings.UnknownAddStrategy)
                };
            }
        }
    }
}
