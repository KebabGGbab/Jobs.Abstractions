namespace Jobs.Abstractions
{
    public class JobContext
    {
        public static readonly JobContext Empty = new();

        public JobContext()
        {
        }
    }
}
