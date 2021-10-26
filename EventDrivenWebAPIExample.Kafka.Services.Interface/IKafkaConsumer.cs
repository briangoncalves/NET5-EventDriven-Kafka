using System.Threading.Tasks;
using Kafka.Public;

namespace EventDrivenWebAPIExample.Kafka.Services.Interface
{
    public interface IKafkaConsumer
    {
        Task Consume(RawKafkaRecord record, IMappedServices mappedServices);
    }
}
