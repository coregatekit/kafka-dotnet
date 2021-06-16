using ConsumerService.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace ConsumerService.Services
{
  public class BookService
  {
    private readonly IMongoCollection<Book> _books;

    public BookService(IBookstoreDatabaseSettings settings)
    {
      var client = new MongoClient(settings.ConnectionString);
      var database = client.GetDatabase(settings.DatabaseName);

      _books = database.GetCollection<Book>(settings.BooksCollectionName);
    }

    public List<Book> GetBooks() =>
      _books.Find(book => true).ToList();

    public Book GetBook(string code) =>
      _books.Find<Book>(book => book.Code == code).FirstOrDefault();

    public async Task Create(Book request)
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
      book.UpdatedDate = request.UpdatedDate;
      await _books.InsertOneAsync(book);
    }

    public async Task Update(string code, Book request)
    {
      // consume
      var book = new Book();
      book.Code = request.Code;
      book.Name = request.Name;
      book.Price = request.Price;
      book.Category = request.Category;
      book.Author = request.Author;
      book.CreateBy = request.CreateBy;
      book.UpdateBy = request.UpdateBy;
      book.CreatedDate = request.CreatedDate;
      book.UpdatedDate = request.UpdatedDate;
      await _books.ReplaceOneAsync(book => book.Code == code, book);
    }
  }
}