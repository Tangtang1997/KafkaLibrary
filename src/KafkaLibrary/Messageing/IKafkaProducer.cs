namespace KafkaLibrary.Messageing;

public interface IKafkaProducer<in T> where T : IKafkaRequest
{
    KafkaConfiguration KafkaConfiguration { get; }

    Task PublishAsync(string topic, T message, CancellationToken cancellationToken = default);

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
} 