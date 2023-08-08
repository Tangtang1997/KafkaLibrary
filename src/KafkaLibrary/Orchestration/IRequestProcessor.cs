namespace KafkaLibrary.Orchestration;

public interface IRequestProcessor<in T> where T : IKafkaRequest
{
    Task ProcessorAsync(T request);

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}