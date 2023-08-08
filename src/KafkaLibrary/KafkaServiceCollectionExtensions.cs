namespace KafkaLibrary;

public static class KafkaServiceCollectionExtensions
{
    public static IServiceCollection AddKafka<TRequest, TRequestProcessor>(
        this IServiceCollection services,
        Action<KafkaOptions> action)
        where TRequest : IKafkaRequest
        where TRequestProcessor : IRequestProcessor<TRequest>
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var options = new KafkaOptions();
        action(options);

        services.AddKafka<TRequest, TRequestProcessor>(options.KafkaConfiguration);

        return services;
    }

    public static IServiceCollection AddKafka<TRequest, TRequestProcessor>(
        this IServiceCollection services,
        KafkaConfiguration kafkaConfiguration)
        where TRequest : IKafkaRequest
        where TRequestProcessor : IRequestProcessor<TRequest>
    {
        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();

        services.TryAddSingleton<IKafkaProducer<TRequest>>(_ => new KafkaProducer<TRequest>(kafkaConfiguration, loggerFactory.CreateLogger<KafkaProducer<TRequest>>()));
        services.TryAddSingleton<IKafkaConsumer<TRequest>>(_ => new KafkaConsumer<TRequest>(kafkaConfiguration, loggerFactory.CreateLogger<KafkaConsumer<TRequest>>()));
        services.TryAddSingleton<IRequestConsumer<TRequest>, RequestConsumer<TRequest>>();
        services.TryAddSingleton(typeof(IRequestProcessor<TRequest>), typeof(TRequestProcessor));
        services.TryAddSingleton<IConsumerProcessorManager<TRequest>, ConsumerProcessorManager<TRequest>>();

        return services;
    }
}