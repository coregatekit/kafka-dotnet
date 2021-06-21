using System;
using Microsoft.Extensions.Hosting;

using Confluent.Kafka;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ConsumerService.Models;

namespace ConsumerService.Services
{
    public class KafakConsumerService : BackgroundService
    {
        private readonly string topic = "bookstore_example";
        private readonly string groupId = "bookstore_exmaple_group";
        private readonly string host = "localhost:9092";
        private readonly BookService _bookService;

        public KafakConsumerService(BookService bookService)
        {
            _bookService = bookService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => StartConsumer(stoppingToken));
            return Task.CompletedTask;
        }

        private Task StartConsumer(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = host,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            Console.WriteLine("Kafka consumer starting");

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    while (true)
                    {
                        consumer.Subscribe(topic);
                        var consumeResult = consumer.Consume();
                        if (consumeResult != null)
                        {
                            OnConsumerRecieved(consumeResult);
                            Console.WriteLine($"Received message {consumeResult.Message.Value}");
                        }
                    }
                }
            }
            return Task.CompletedTask;
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
    }
}