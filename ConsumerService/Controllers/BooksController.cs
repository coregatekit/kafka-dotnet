using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConsumerService.Models;
using ConsumerService.Services;

namespace ConsumerService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BooksController : ControllerBase
  {
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
      _bookService = bookService;
    }

    [HttpGet]
    public IActionResult GetBooks()
    {
      var result = _bookService.GetBooks();

      return Ok(result);
    }

    [HttpGet("{code}")]
    public IActionResult GetBookByCode(string code)
    {
      var result = _bookService.GetBook(code);

      return Ok(result);
    }
  }
}