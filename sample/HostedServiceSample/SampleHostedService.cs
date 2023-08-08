namespace HostedServiceSample;

public class SampleHostedService : IHostedService
{
    private readonly IConsumerProcessorManager<TestKafkaRequest> _testConsumerProcessorManager;
    private readonly ILogger<SampleHostedService> _logger;

    private Task tesTask = null!;

    public SampleHostedService(
        IConsumerProcessorManager<TestKafkaRequest> testConsumerProcessorManager,
        ILogger<SampleHostedService> logger)
    {
        _testConsumerProcessorManager = testConsumerProcessorManager;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting SampleHostedService");

        tesTask = Task.Run(() => _testConsumerProcessorManager.StartAsync(cancellationToken), cancellationToken);
        Task.WaitAll(tesTask);

        ////如果有多个kafka配置，使用Task.WaitAll()等待所有的kafka配置都启动完成
        //Task.WaitAll(task1, task2, task3);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping SampleHostedService");

        tesTask = Task.Run(() => _testConsumerProcessorManager.StopAsync(cancellationToken), cancellationToken);
        Task.WaitAll(tesTask);

        ////如果有多个kafka配置，使用Task.WaitAll()等待所有的kafka配置都启动完成
        //Task.WaitAll(task1, task2, task3);

        return Task.CompletedTask;
    }
}