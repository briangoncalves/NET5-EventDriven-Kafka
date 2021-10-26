using System.Collections.Generic;
using EventDrivenWebAPIExample.Kafka.Infrastructure.Interface;

namespace EventDrivenWebAPIExample.Kafka.Infrastructure
{
    public class KafkaListener : IKafkaListener
    {
        public string ConsumerGroupId { get; set; }
        public IEnumerable<string> Topics { get; set; }
    }
}
