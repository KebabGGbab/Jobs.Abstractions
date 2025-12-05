using Jobs.Abstractions.Resources;

namespace Jobs.Abstractions
{
    public abstract class Job
    {
        public StatusJob Status { get; protected set; }
        public bool? IsSuccess { get; protected set; }

        public event EventHandler? Completed;

        public async Task ExecuteAsync(RunArgs args, CancellationToken? cancellation = null)
        {
            if (Status != StatusJob.None)
            {
                throw new InvalidOperationException(Strings.CallCompletedJob);
            }

            Status = StatusJob.Proccess;

            try
            {
                await RunAsync(args, cancellation).ConfigureAwait(false);
                Complete(true);
            }
            catch (OperationCanceledException)
            {
                Complete(false);
            }
            catch
            {
                Complete(false);
                throw;
            }
        }

        protected abstract Task RunAsync(RunArgs args, CancellationToken? cancellation);

        protected void Complete(bool success)
        {
            if (Status is StatusJob.Completed or StatusJob.Canceled)
            {
                return;
            }

            IsSuccess = success;
            Status = success ? StatusJob.Completed : StatusJob.Canceled;

            Completed?.Invoke(this, EventArgs.Empty);
        }
    }
}
