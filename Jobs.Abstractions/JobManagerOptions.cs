namespace Jobs.Abstractions
{
    public class JobManagerOptions
    {
        public bool Disposable { get; set; }
        public AddStrategy AddStrategy { get; set; } = AddStrategy.Deferred;
        public bool Clean { get; set; } = true;
    }
}
