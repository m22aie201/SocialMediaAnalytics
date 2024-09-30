using Confluent.Kafka;

namespace DistributionService
{
    public class Worker : BackgroundService
    {
        List<MessageStructure> messageList = new List<MessageStructure>()
        {
            new MessageStructure()
            {
                MessageId = Guid.NewGuid(),
                Message = "New Post",
                ServiceType = "Post"
            },
            new MessageStructure()
            {
                MessageId = Guid.NewGuid(),
                Message = "Comment Received",
                ServiceType = "Comment"
            }
        };


        public Worker(ILogger<Worker> logger)
        {

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Configure producer
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                DeliveryResult<Null, string>? result = null;

                foreach (var message in messageList)
                {
                    // Check for message type of incoming messages, and distribute message accordingly.
                    switch (message.ServiceType)
                    {
                        case "Post":
                            result = await producer.ProduceAsync("message-distribution-posts", new Message<Null, string> { Value = "Message Distribution - Post" });
                            break;
                        case "Comment":
                            result = await producer.ProduceAsync("message-distribution-comments", new Message<Null, string> { Value = "Message Distribution - Comment" });
                            break;
                    }

                    // Print message sent status
                    if(result != null)
                    {
                        Console.WriteLine($"Message sent status => {result.Status}");
                        Console.WriteLine($"Partition           => {result.Partition}");
                        Console.WriteLine($"Offset              => {result.Offset}");
                        Console.WriteLine(result.Topic);
                    }
                }
            }
        }
    }
}