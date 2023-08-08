var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(config =>
    {
        config.AddEnvironmentVariables();
    })
   .ConfigureAppConfiguration((hostContext, config) =>
   {
       config.AddJsonFile("appsettings.json");

       if (hostContext.HostingEnvironment.IsDevelopment())
       {
           config.AddJsonFile("appsettings.developer.json");
       }

       if (hostContext.HostingEnvironment.IsProduction())
       {
           config.AddJsonFile("appsettings.production.json");
       }

       config.AddEnvironmentVariables();
   })
   .ConfigureServices((context, services) =>
   {
       //添加Serilog
       var configuration = context.Configuration;
       var logger = new LoggerConfiguration()
           .ReadFrom.Configuration(context.Configuration)
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();

       services.AddLogging(loggerBuilder =>
       {
           loggerBuilder.ClearProviders();
           loggerBuilder.AddSerilog(logger);
       });


       services.AddKafka<TestKafkaRequest, TestKafkaRequestProcessor>(options =>
       {
           options.KafkaConfiguration = configuration.GetSection("Kafka").Get<KafkaConfiguration>() ?? throw new Exception("Kafka configuration is null");
       });

       services.AddHostedService<SampleHostedService>();
   })
    .Build();

await host.RunAsync();
