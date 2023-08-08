namespace KafkaLibrary.Messageing;

public interface IKafkaRequest
{
    public int RetryCount { get; set; }
}