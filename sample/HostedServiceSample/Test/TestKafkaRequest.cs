namespace HostedServiceSample.Test;

public class TestKafkaRequest : IKafkaRequest
{
    public int RetryCount { get; set; }

    #region 自定义消息结构

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    #endregion
}