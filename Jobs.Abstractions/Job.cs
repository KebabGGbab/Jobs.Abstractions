using Jobs.Abstractions.Resources;

namespace Jobs.Abstractions
{
    public abstract class Job
    {
        public StatusJob Status { get; private set; }

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
                Complete(null);
            }
            catch
            {
                Complete(false);
                throw;
            }
        }

        protected abstract Task RunAsync(RunArgs args, CancellationToken? cancellation);

        private void Complete(bool? success)
        {
            Status = success switch
            {
                true => StatusJob.Completed,
                false => StatusJob.Unsuccessful,
                null => StatusJob.Canceled
            };

            Completed?.Invoke(this, EventArgs.Empty);
        }
    }
}
