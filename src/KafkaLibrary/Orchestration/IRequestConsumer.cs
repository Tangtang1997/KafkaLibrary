namespace KafkaLibrary.Orchestration;

public interface IRequestConsumer<T> where T : IKafkaRequest
{
    event Func<T,Task>  RequestReceivedAsync;

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task RetryAsync(T request);
}