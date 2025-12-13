namespace Jobs.Abstractions
{
    public class JobManagerOptions
    {
        public bool Disposable { get; set; }
        public AddHandler AddStrategy { get; set; } = AddHandler.Deferred;
        public bool Clean { get; set; } = true;
    }
}
