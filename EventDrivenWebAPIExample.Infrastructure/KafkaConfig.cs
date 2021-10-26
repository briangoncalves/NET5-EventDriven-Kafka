using System.Collections.Generic;
using EventDrivenWebAPIExample.Domain;
using EventDrivenWebAPIExample.Infrastructure.Interface;

namespace EventDrivenWebAPIExample.Infrastructure
{
    public class KafkaConfig : IKafkaConfig
    {
        public string Host { get; set; }
        public IEnumerable<IKafkaSender> Sender { get; set; }
        public string Source { get; set; }
    }
}
