using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsumerService.Models
{
  public class Book
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Code { get; set; }
    [BsonElement("Name")]
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public string Author { get; set; }
    public string CreateBy { get; set; }
    public string UpdateBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
  }
}