using Jobs.Abstractions.Resources;

namespace Jobs.Abstractions
{
    public abstract class JobManager
    {
        private readonly List<Job> _jobs = [];
        private readonly List<Job>? _queue;
        private readonly bool _isDisposable;
        private readonly bool _clean;

        private AddHandler _addHandler;
        private int _progress;

        public int Progress => _progress;
        public bool IsProcessing { get; protected set; }
        public IReadOnlyList<Job> Jobs => _jobs;

        public event EventHandler? Completed;
        public event EventHandler? JobAdded;
        
        public JobManager() : this(new JobManagerOptions()) { }

        public JobManager(JobManagerOptions options)
        { 
            _isDisposable = options.Disposable;
            _clean = options.Clean;

            if (!_isDisposable)
            {
                _queue = [];
            }

            _addHandler = options.AddStrategy;
        }

        public JobManager(Job job) : this([job]) { }

        public JobManager(IEnumerable<Job> jobs , JobManagerOptions? options = null) : this(options ?? new JobManagerOptions())
        {
            ArgumentNullException.ThrowIfNull(jobs, nameof(jobs));
            _jobs.Capacity = jobs.Count();
            _jobs.AddRange(jobs);
        }

        public async Task ExecuteJobsAsync()
        {
            if (IsProcessing)
            {
                throw new InvalidOperationException(Strings.CallJobManagerInProccess);
            }

            IsProcessing = true;

            try
            {
                await RunJobsAsync().ConfigureAwait(false);
            }
            finally
            {
                IsProcessing = false;
                Complete();
            }
        }

        public abstract Task RunJobsAsync();

        public void AddJob(Job job)
        {
            bool notify = _addHandler.Add(_jobs, _isDisposable, job, IsProcessing, _queue);

            if (notify)
            {
                JobAdded?.Invoke(this, EventArgs.Empty);
            }
        }

        public abstract void Cancel();

        public void ChangeAddStrategy(AddHandler strategy)
        {
            ArgumentNullException.ThrowIfNull(strategy, nameof(strategy));

            _addHandler = strategy;
        }

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



        protected virtual void OnJobCompleted(object? sender, EventArgs e)
        {
            Interlocked.Increment(ref _progress);
        }
    }
}
