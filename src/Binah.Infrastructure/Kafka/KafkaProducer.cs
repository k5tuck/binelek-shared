using Confluent.Kafka;
using System.Text.Json;

namespace Binah.Infrastructure.Kafka;

public class KafkaProducer
{
    private readonly IProducer<string, string> _producer;
    
    public KafkaProducer(string bootstrapServers)
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }
    
    public async Task ProduceAsync<T>(string topic, T message, string? key = null)
    {
        var json = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(topic, new Message<string, string> 
        { 
            Key = key ?? Guid.NewGuid().ToString(), 
            Value = json 
        });
    }
}
