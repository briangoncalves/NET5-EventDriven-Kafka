using System.Threading.Tasks;

namespace EventDrivenWebAPIExample.Kafka.Infrastructure.Interface.Mongo
{
    public interface IConnectionThrottlingPipeline
    {
        Task<T> AddRequest<T>(Task<T> task);

        Task AddRequest(Task task);
    }
}
