namespace KafkaLibrary.Orchestration;

public class ConsumerProcessorManager<T> : IConsumerProcessorManager<T> where T : IKafkaRequest
{
    protected IRequestConsumer<T> Consumer;
    protected IRequestProcessor<T> Processor;

    private readonly ILogger<ConsumerProcessorManager<T>> _logger;
    private readonly int _maxRequestRetryCount;

    public ConsumerProcessorManager(
        IRequestConsumer<T> consumer, 
        IRequestProcessor<T> processor,
        ILogger<ConsumerProcessorManager<T>> logger,
        int maxRequestRetryCount = 0)
    { 
        Processor = processor;
        Consumer = consumer;

        _logger = logger;
        _maxRequestRetryCount = maxRequestRetryCount;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting ConsumerProcessorManager");

        try
        {
            await Processor.StartAsync(cancellationToken);
            Consumer.RequestReceivedAsync += OnRequestReceivedAsync;
            await Consumer.StartAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error starting ConsumerProcessorManager: {e.Message}");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping ConsumerProcessorManager");

        try
        {
            Consumer.RequestReceivedAsync -= OnRequestReceivedAsync;
            await Consumer.StopAsync(cancellationToken);
            await Processor.StopAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error stopping ConsumerProcessorManager: {e.Message}");
        }
    }

    private async Task OnRequestReceivedAsync(T request)
    {
        try
        {
            _logger.LogInformation("Processing request");

            await Processor.ProcessorAsync(request);

            _logger.LogInformation("Processing request completed");
        }
        catch (Exception e)
        {
            if (request.RetryCount >= _maxRequestRetryCount)
            {
                _logger.LogError(e, $"Error processing request: {e.Message}"); 
                return;
            }

            request.RetryCount++;
            await Consumer.RetryAsync(request);

            _logger.LogError(e, $"Redelivered processing request: {e.Message}");
        }
    }
}