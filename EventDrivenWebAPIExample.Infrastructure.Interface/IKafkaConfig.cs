using System.Collections.Generic;
using EventDrivenWebAPIExample.Domain;

namespace EventDrivenWebAPIExample.Infrastructure.Interface
{
    public interface IKafkaConfig
    {
        string Host { get; set; }
        IEnumerable<IKafkaSender> Sender { get; set; }
        string Source { get; set; }
    }
}
