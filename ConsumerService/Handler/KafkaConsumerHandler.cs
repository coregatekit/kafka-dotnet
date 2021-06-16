using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using ConsumerService.Models;
using ConsumerService.Services;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace ConsumerService.Handler
{
  public class KafkaConsumerHandler : IHostedService
  {
    private readonly string topic = "bookstore_example";
    private readonly string groupId = "bookstore_exmaple_group";
    private readonly string host = "localhost:9092";
    private readonly BookService _bookService;

    public KafkaConsumerHandler(BookService bookService)
    {
      _bookService = bookService;
    }

    private async void OnConsumerRecieved(ConsumeResult<Ignore, string> consume)
    {
      var kafka = JsonConvert.DeserializeObject<KafkaConsume>(consume.Message.Value);
      var book = JsonConvert.DeserializeObject<Book>(kafka.Message);
      if (kafka.Action == "ADD")
      {
        await _bookService.Create(book);
      }
      else
      {
        await _bookService.Update(book.Code, book);
      }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      Console.WriteLine("Waiting for consume message.");
      var conf = new ConsumerConfig
      {
        GroupId = groupId,
        BootstrapServers = host,
        AutoOffsetReset = AutoOffsetReset.Earliest
      };
      using (var builder = new ConsumerBuilder<Ignore, string>(conf).Build())
      {
        builder.Subscribe(topic);
        var cancelToken = new CancellationTokenSource();
        try
        {
          while (true)
          {
            var consumer = builder.Consume(cancelToken.Token);
            OnConsumerRecieved(consumer);
            Console.WriteLine($"Received message {consumer.Message.Value}");
          }
        }
        catch (Exception)
        {
          builder.Close();
        }
      }
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }
  }
}