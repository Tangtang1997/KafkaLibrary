namespace KafkaLibrary.Dispatch;

public class RequestConsumer<T> : IRequestConsumer<T> where T : IKafkaRequest
{
    private readonly IKafkaConsumer<T> _concumer;
    private readonly IKafkaProducer<T> _producer;

    public event Func<T, Task>? RequestReceivedAsync;

    public RequestConsumer(IKafkaConsumer<T> concumer, IKafkaProducer<T> producer)
    {
        _concumer = concumer;
        _concumer.RequestReceivedAsync += OnRequestReceivedAsync;
        _producer = producer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _concumer.StartAsync(cancellationToken);
        await _producer.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _concumer.RequestReceivedAsync -= OnRequestReceivedAsync;
        await _concumer.StopAsync(cancellationToken);
        await _producer.StopAsync(cancellationToken);
    }

    public async Task RetryAsync(T request) 
    {
        await _producer.PublishAsync(_concumer.KafkaConfiguration.Topic, request);
    }

    private async Task OnRequestReceivedAsync(T request)
    {
        await RequestReceivedAsync?.Invoke(request)!;
    }
}