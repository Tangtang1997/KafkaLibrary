namespace KafkaLibrary.Messageing;

public class KafkaJsonDeserializer<T> : IDeserializer<T>
{
    private readonly ILogger<KafkaJsonDeserializer<T>> _logger;

    public KafkaJsonDeserializer(ILogger<KafkaJsonDeserializer<T>> logger)
    {
        _logger = logger;
    }

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            return default!;
        }

        try
        {
            var json = Encoding.UTF8.GetString(data);
             
            try
            {
                var t = JsonSerializer.Deserialize<T>(json);
                return t!; 
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Consumer message deserialize error: {json}");
                throw;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Consumer message decoding error");
            throw;
        }
    }
}