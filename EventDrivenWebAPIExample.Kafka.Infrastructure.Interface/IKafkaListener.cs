using System.Collections.Generic;

namespace EventDrivenWebAPIExample.Kafka.Infrastructure.Interface
{
    public interface IKafkaListener
    {
        string ConsumerGroupId { get; set; }
        IEnumerable<string> Topics { get; set; }
    }
}
