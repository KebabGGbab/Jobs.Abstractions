using Jobs.Abstractions.Resources;
using KebabGGbab.ComponentModel;

namespace Jobs.Abstractions
{
    public abstract class JobManager<T> : ObservableObject where T : Job
    {
        private readonly List<T> _jobs = [];
        private readonly List<T>? _queue;
        private readonly bool _isDisposable;
        private readonly bool _clean;
        private readonly Lock _lock = new();
       
        private AddHandler _addHandler;

        public int Progress
        {
            get => field;
            private set => OnPropertyChanged(ref field, value); 

        }

        public bool IsProcessing 
        { 
            get => field;
            private set => OnPropertyChanged(ref field, value); 
        }

        public IReadOnlyList<T> Jobs => _jobs;

        public event EventHandler? Completed;
        public event EventHandler? JobAdded;
        
        public JobManager() 
            : this(new JobManagerOptions())
        {
        }

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

        public JobManager(IEnumerable<T> jobs, JobManagerOptions? options = null)
            : this(options ?? new JobManagerOptions())
        {
            ArgumentNullException.ThrowIfNull(jobs, nameof(jobs));

            _jobs.AddRange(jobs);

            foreach (Job job in jobs)
            {
                job.Completed += OnJobCompleted;
            }

        }

        public async Task ExecuteJobsAsync(RunArgs args)
        {
            if (IsProcessing)
            {
                throw new InvalidOperationException(Strings.CallJobManagerInProccess);
            }

            IsProcessing = true;

            try
            {
                await RunJobsAsync(args).ConfigureAwait(false);
            }
            finally
            {
                IsProcessing = false;
                Complete();
            }
        }

        public abstract Task RunJobsAsync(RunArgs args);

        public void AddJob(T job)
        {
            if (_addHandler.Add(_jobs, _isDisposable, job, IsProcessing, _queue))
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

        private void OnJobCompleted(object? sender, EventArgs e)
        {
            lock (_lock)
            {
                Progress++;
            }

            if (_isDisposable)
            {
                ((Job)sender!).Completed -= OnJobCompleted;
            }
        }
    }
}
