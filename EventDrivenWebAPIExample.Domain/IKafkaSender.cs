namespace EventDrivenWebAPIExample.Domain
{
    public interface IKafkaSender
    {
        string Topic { get; set; }
    }
}
