namespace KafkaLibrary.Messageing;

public abstract class KafkaBase : IDisposable
{
    private readonly KafkaConfiguration _configuration;

    protected KafkaBase(KafkaConfiguration configuration)
    {
        _configuration = configuration;
    }

    public virtual Task StopAsync(CancellationToken cancellationToken) 
    {
         

        return Task.CompletedTask;
    }

    protected virtual void CheckNullValue()
    {
        if (string.IsNullOrEmpty(_configuration.BootstrapServers))
        {
            throw new ArgumentNullException(nameof(_configuration.BootstrapServers), "Kafka configuration: BootstrapServers is null");
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}