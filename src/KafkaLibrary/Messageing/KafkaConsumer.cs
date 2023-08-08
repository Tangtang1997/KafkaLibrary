namespace KafkaLibrary.Messageing;

public class KafkaConsumer<T> : KafkaBase, IKafkaConsumer<T> where T : IKafkaRequest
{
    private readonly ILogger<KafkaConsumer<T>> _logger;

    private IConsumer<Ignore, T> _consumer = null!;

    public KafkaConfiguration KafkaConfiguration { get; }
    public event Func<T, Task> RequestReceivedAsync = null!;

    public KafkaConsumer(
        KafkaConfiguration configuration,
        ILogger<KafkaConsumer<T>> logger)
        : base(configuration) 
    {
        KafkaConfiguration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("KafkaConsumer started");

        await ConsumeAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _consumer.Close();

            _logger.LogInformation("KafkaConsumer stopped");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error stopping KafkaConsumer: {e.Message}");
        }

        await Task.CompletedTask;
    }

    protected sealed override void CheckNullValue()
    {
        base.CheckNullValue();

        if (string.IsNullOrEmpty(KafkaConfiguration.Topic))
        {
            throw new ArgumentNullException(nameof(KafkaConfiguration.Topic), "Kafka configuration: Topic is null");
        }
    }

    private async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        try
        {
            InitConnect();

            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);

                if (consumeResult.IsPartitionEOF)
                {
                    _logger.LogInformation($"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");
                    continue;
                }

                await OnReceiveCompletedAsync(consumeResult);
            }
        }
        catch (ConsumeException e)
        {
            _logger.LogError($"Error consuming message: {e.Error.Reason}");
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer cancelled");
            _consumer.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error consuming message: {ex.Message}");
        }
    }

    public void InitConnect()
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = KafkaConfiguration.BootstrapServers,
            GroupId = KafkaConfiguration.GroupId,
            StatisticsIntervalMs = KafkaConfiguration.StatisticsIntervalMs,
            SessionTimeoutMs = KafkaConfiguration.SessionTimeoutMs,
            AllowAutoCreateTopics = KafkaConfiguration.AllowAutoCreateTopics,
            EnablePartitionEof = KafkaConfiguration.EnablePartitionEof,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumerBuilder = new ConsumerBuilder<Ignore, T>(consumerConfig).SetErrorHandler((consumer, e) =>
            {
                _logger.LogError($"- [{consumer.Name}] -> Error: {e.Reason}");
            })
            .SetStatisticsHandler((consumer, _) =>
            {
                _logger.LogInformation($"- [{consumer.Name}] -> 消息监听中...... ");
            })
            .SetPartitionsAssignedHandler((consumer, partitions) =>
            {
                _logger.LogInformation($" - [{consumer.Name}] -> 分区: {string.Join(", ", partitions)}");
            })
            .SetValueDeserializer(new KafkaJsonDeserializer<T>(NullLogger<KafkaJsonDeserializer<T>>.Instance));

        _consumer = consumerBuilder.Build();

        _consumer.Subscribe(KafkaConfiguration.Topic);
    }

    public async Task OnReceiveCompletedAsync(ConsumeResult<Ignore, T> consumeResult)
    {
        try
        {
            var data = consumeResult.Message.Value;

            _logger.LogInformation("接收到消息: {0}", data);

            var isFired = await FireReceiveEventAsync(data);
            if (isFired)
            {
                _consumer.Commit(consumeResult);
                _logger.LogInformation("Message commited. ");
            }
            else
            {
                _logger.LogError("Error consuming message: {0}", data);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Error consuming message: {e.Message}");
        }
    }

    private async Task<bool> FireReceiveEventAsync(T data)
    {
        try
        {
            await RequestReceivedAsync.Invoke(data);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error consuming message: {ex.Message}");
            return false;
        }
    }
}