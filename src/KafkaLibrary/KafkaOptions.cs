namespace KafkaLibrary;

public class KafkaOptions
{
    public KafkaOptions()
    {

    }

    public KafkaOptions(KafkaConfiguration kafkaConfiguration)
    {
        KafkaConfiguration = kafkaConfiguration;
    }

    public KafkaConfiguration KafkaConfiguration { get; set; } = null!;
}