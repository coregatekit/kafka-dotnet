using System;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using ProducerService.Models;

namespace ProducerService.Services
{
  public class KafkaProducerService
  {
    private readonly ProducerConfig config;
    private readonly string topic;

    public KafkaProducerService(IOptions<KafkaConfigs> kafkaConfigs)
    {
        config = new ProducerConfig {
          BootstrapServers = kafkaConfigs.Value.Host
        };
        topic = kafkaConfigs.Value.Topic;
    }

    public void SendToKafka(string message)
    {
      using (var producer = new ProducerBuilder<Null, string>(config).Build())
      {
        try
        {
          producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
          Console.WriteLine($"Produce message {message}");
        }
        catch (Exception e)
        {
          Console.WriteLine($"Oops, something went wrong: {e}");
        }
      }
    }
  }
}