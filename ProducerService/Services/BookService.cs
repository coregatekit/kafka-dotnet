using ProducerService.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace ProducerService.Services
{
    public class BookService
    {
      private readonly IMongoCollection<Book> _books;
      private readonly KafkaProducerService _kafka;

      public BookService(IBookstoreDatabaseSettings settings, KafkaProducerService kafka)
      {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        _books = database.GetCollection<Book>(settings.BooksCollectionName);

        _kafka = kafka;
      }

      public async Task<Book> Create(Book request)
      {
        var book = new Book();
        book.Code = Guid.NewGuid().ToString();
        book.Name = request.Name;
        book.Price = request.Price;
        book.Category = request.Category;
        book.Author = request.Author;
        book.CreateBy = request.CreateBy;
        book.UpdateBy = request.UpdateBy;
        book.CreatedDate = DateTime.Now;
        book.UpdatedDate = DateTime.Now;
        await _books.InsertOneAsync(book);

        // publish
        var prepareBook = JsonSerializer.Serialize(book);
        var kafka = new KafkaProduce();
        kafka.Action = "ADD";
        kafka.Message = prepareBook;
        _kafka.SendToKafka(JsonSerializer.Serialize(kafka));

        return book;
      }

      public async Task Update(string code, Book request)
      {
        var book = new Book();
        book.Code = request.Code;
        book.Name = request.Name;
        book.Price = request.Price;
        book.Category = request.Category;
        book.Author = request.Author;
        book.CreateBy = request.CreateBy;
        book.UpdateBy = request.UpdateBy;
        book.CreatedDate = request.CreatedDate;
        book.UpdatedDate = DateTime.Now;;
        await _books.InsertOneAsync(book);

        // publish
        var prepareBook = JsonSerializer.Serialize(book);
        var kafka = new KafkaProduce();
        kafka.Action = "UPDATE";
        kafka.Message = prepareBook;
        _kafka.SendToKafka(JsonSerializer.Serialize(kafka));
      }
    }
}