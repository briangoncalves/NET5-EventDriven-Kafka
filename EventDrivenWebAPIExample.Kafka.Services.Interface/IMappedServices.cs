using System.Collections.Generic;
using EventDrivenWebAPIExample.Kafka.Domain;

namespace EventDrivenWebAPIExample.Kafka.Services.Interface
{
    public interface IMappedServices
    {
        Dictionary<string, MessageTypeServiceMap> Services { get; set; }
    }
}
