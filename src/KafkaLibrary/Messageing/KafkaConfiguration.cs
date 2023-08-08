namespace KafkaLibrary.Messageing;

public class KafkaConfiguration
{
    public string BootstrapServers { get; set; } = string.Empty;

    public string Topic { get; set; } = string.Empty;

    public string GroupId { get; set; } = string.Empty;

    public bool EnableAutoCommit { get; set; }

    public int StatisticsIntervalMs { get; set; } = 5000;

    public int SessionTimeoutMs { get; set; } = 6000;

    public bool AllowAutoCreateTopics { get; set; } = true;

    public bool EnablePartitionEof { get; set; } = true;
}