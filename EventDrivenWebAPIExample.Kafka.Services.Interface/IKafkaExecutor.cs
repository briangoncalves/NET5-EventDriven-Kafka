using System.Threading.Tasks;

namespace EventDrivenWebAPIExample.Kafka.Services.Interface
{
    public interface IKafkaExecutor<T> where T : class
    {
        Task<bool> Execute(T message, string subject);
    }
}
