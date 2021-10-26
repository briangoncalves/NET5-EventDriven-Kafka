using System;
using Confluent.Kafka;

namespace EventDrivenWebAPIExample.Domain
{
    public enum ReturnStatus
	{
		Success = 0,
		ErrorUnknown = -1,
		ErrorHostNotFound = -2
	}

	public class KafkaReturnValue
	{
		public ReturnStatus Status { get; set; }
		public Exception Exception { get; set; }
		public DeliveryResult<string, string> DeliveryResult { get; set; }
	}
}
