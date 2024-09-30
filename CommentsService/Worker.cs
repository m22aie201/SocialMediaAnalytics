using Confluent.Kafka;
using System.Runtime.CompilerServices;
using static Confluent.Kafka.ConfigPropertyNames;

namespace CommentsService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        Dictionary<string, int> eventCounts = new Dictionary<string, int>();

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var totalCounts = 0;
            
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "CommentsApplication",
                AutoOffsetReset = AutoOffsetReset.Latest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                //Subscribe to only post messages
                consumer.Subscribe("message-distribution-comments");

                while (!stoppingToken.IsCancellationRequested)
                {
                    var message = consumer.Consume(100);
                    
                    if (message != null)
                    {
                        var offset = message.Offset;

                        var consumerMessage = message.Message.Value;
                        Console.WriteLine(consumerMessage);

                        // Commit kafka message if successfully processed
                        consumer.Commit(message);

                        var producer = CreatePublisher().Build();

                        //Send message to different service to check increasing popularity
                        var result = await producer.ProduceAsync("message-distribution-popularity", new Message<Null, string> { Value = "popularity count" });
                        Console.WriteLine($"Message sent status => {result.Status}");
                        Console.WriteLine($"Partition           => {result.Partition}");
                        Console.WriteLine($"Offset              => {result.Offset}");
                        Console.WriteLine(result.Topic);

                        // Maintain analytics data
                        totalCounts++;

                        // Checking for specific comment
                        if (consumerMessage.Contains("festival"))
                        {
                            if (!eventCounts.ContainsKey("festival"))
                            {
                                eventCounts.Add("festival", 1);
                            }
                            else
                            {
                                eventCounts.Add("festival", eventCounts["festival"]++);
                            }
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }

        // This method can be used to check for analytics
        public Dictionary<string, int> CheckAnalytics()
        {
            return eventCounts;
        }

        // Create producer client config
        private ProducerBuilder<Null, string> CreatePublisher()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };

            return new ProducerBuilder<Null, string>(config);
        }
    }
}