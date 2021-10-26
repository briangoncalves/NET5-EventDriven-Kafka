using System.Collections.Generic;

namespace EventDrivenWebAPIExample.Kafka.Infrastructure.Interface
{
    public interface IKafkaConfig
    {
        string Host { get; set; }
        IEnumerable<IKafkaListener> Listeners { get; set; }
    }
}
