using System.Collections.Generic;
using EventDrivenWebAPIExample.Kafka.Infrastructure.Interface;

namespace EventDrivenWebAPIExample.Kafka.Infrastructure
{
    public class KafkaConfig : IKafkaConfig
    {
        public string Host { get; set; }
        public IEnumerable<IKafkaListener> Listeners { get; set; }
    }
}
