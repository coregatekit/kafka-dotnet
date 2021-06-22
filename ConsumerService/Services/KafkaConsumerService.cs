using System;
using Microsoft.Extensions.Hosting;

using Confluent.Kafka;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ConsumerService.Models;
using Microsoft.Extensions.Options;

namespace ConsumerService.Services
{
    public class KafakConsumerService : BackgroundService
    {
        private readonly string topic;
        private readonly string groupId;
        private readonly string host;
        private readonly BookService _bookService;
        private readonly ConsumerConfig _consumerConfig;

        public KafakConsumerService(BookService bookService, IOptions<KafkaConfigs> kafkaConfigs)
        {
            _bookService = bookService;
            host = kafkaConfigs.Value.Host;
            groupId = kafkaConfigs.Value.GroupId;
            topic = kafkaConfigs.Value.Topic;

            _consumerConfig = new ConsumerConfig {
                BootstrapServers = host,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => StartConsumer(stoppingToken));
            return Task.CompletedTask;
        }

        private Task StartConsumer(CancellationToken cancellationToken)
        {
            Console.WriteLine("Kafka consumer starting");

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
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