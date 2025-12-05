namespace Jobs.Abstractions
{
    public interface IJobManagerFactory
    {
        JobManager Create(JobContext context);
    }
}
