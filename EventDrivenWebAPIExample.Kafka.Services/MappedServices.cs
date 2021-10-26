using System.Collections.Generic;
using EventDrivenWebAPIExample.Kafka.Domain;
using EventDrivenWebAPIExample.Kafka.Services.Interface;

namespace EventDrivenWebAPIExample.Kafka.Services
{
    public class MappedServices : IMappedServices
    {
        public Dictionary<string, MessageTypeServiceMap> Services { get; set; }
    }
}
