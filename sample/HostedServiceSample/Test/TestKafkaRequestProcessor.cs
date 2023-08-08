namespace HostedServiceSample.Test;

public class TestKafkaRequestProcessor : IRequestProcessor<TestKafkaRequest>
{
    private readonly ILogger<TestKafkaRequestProcessor> _logger;

    /// <summary>
    /// 可以使用依赖注入获取其他已注册的服务
    /// </summary>
    /// <param name="logger"></param>
    public TestKafkaRequestProcessor(ILogger<TestKafkaRequestProcessor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 一些初始化操作可以写在这里
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 一些释放资源的操作可以写在这里
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 处理消息
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task ProcessorAsync(TestKafkaRequest request)
    {
        _logger.LogInformation($"开始处理消息: {request.Message}");

        //模拟耗时操作
        await Task.Delay(1000);

        _logger.LogInformation("消息处理完成");
    }
}