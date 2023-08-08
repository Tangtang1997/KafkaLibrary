namespace KafkaLibrary.Messageing;

public interface IKafkaConsumer<out T> where T : IKafkaRequest
{
    KafkaConfiguration KafkaConfiguration { get; }

    event Func<T, Task> RequestReceivedAsync;
     
    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}