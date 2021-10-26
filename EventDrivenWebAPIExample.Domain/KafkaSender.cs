namespace EventDrivenWebAPIExample.Domain
{
    public class KafkaSender : IKafkaSender
    {
        public string Topic { get; set; }
    }
}
