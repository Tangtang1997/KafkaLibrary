namespace KafkaLibrary.Messageing;

public class KafkaProducer<T> : KafkaBase, IKafkaProducer<T> where T : IKafkaRequest
{
    public KafkaConfiguration KafkaConfiguration { get; }

    private readonly ILogger<KafkaProducer<T>> _logger;
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(
        KafkaConfiguration kafkaConfiguration,
        ILogger<KafkaProducer<T>> logger)
        : base(kafkaConfiguration)
    {
        CheckNullValue();

        KafkaConfiguration = kafkaConfiguration;
        _logger = logger;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = KafkaConfiguration.BootstrapServers,
            AllowAutoCreateTopics = KafkaConfiguration.AllowAutoCreateTopics
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task PublishAsync(string topic, T message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Publishing message to topic {topic}: {message}");

        try
        {
            var kafkaMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(message)
            };

            await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);

            _logger.LogInformation($"Published message to topic {topic}");
        }
        catch (OperationCanceledException e)
        {
            _logger.LogError(e, $"Operation canceled publishing message to topic {topic}: {e.Message}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error publishing message to topic {topic}: {e.Message}");
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting KafkaProducer");

        if (_producer == null)
        {
            throw new Exception("Producer is null");
        }

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping KafkaProducer");

        try
        {
            _producer.Dispose();

            base.StopAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error stopping KafkaProducer: {e.Message}");
        }

        return Task.CompletedTask;
    }

    protected sealed override void CheckNullValue()
    {
        base.CheckNullValue();
    }
}