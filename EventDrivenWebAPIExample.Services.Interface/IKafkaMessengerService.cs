using System.Collections.Generic;
using System.Threading.Tasks;
using EventDrivenWebAPIExample.Domain;

namespace EventDrivenWebAPIExample.Services.Interface
{
    public interface IKafkaMessengerService
	{
		Task<List<KafkaReturnValue>> SendKafkaMessage(string id, string subject, object message, string topic = "");
	}
}
