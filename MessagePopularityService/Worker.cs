using Confluent.Kafka;

namespace MessagePopularityService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    GroupId = "PopularityApplication",
                    AutoOffsetReset = AutoOffsetReset.Latest // Check for latest kafka offsets only
                };

                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    //Subscribe to only post messages
                    consumer.Subscribe("message-distribution-popularity");

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var message = consumer.Consume(100);

                        if (message != null)
                        {
                            var offset = message.Offset;
                            Console.WriteLine(message.Message);

                            // Commit kafka message if successfully processed
                            consumer.Commit(message);
                        }
                    }
                }

                await Task.CompletedTask;
            }
        }
    }
}