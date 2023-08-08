namespace KafkaLibrary.Orchestration;

public interface IConsumerProcessorManager<T> where T : IKafkaRequest
{
    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
} 