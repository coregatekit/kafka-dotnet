using System;
using System.Text.Json;
using Confluent.Kafka;
using ProducerService.Models;

namespace ProducerService.Services
{
  public class KafkaProducerService
  {
    private readonly ProducerConfig config = new ProducerConfig
    {
      BootstrapServers = "localhost:9092"
    };
    private readonly string topic = "bookstore_example";

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