using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProducerService.Models;
using ProducerService.Services;

namespace ProducerService.Controllers
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

    [HttpPost]
    public async Task<IActionResult> Create(Book book)
    {
      var result = await _bookService.Create(book);

      return Ok(result);
    }

    [HttpPost("Update/{code}")]
    public async Task<IActionResult> Update(string code, Book book)
    {
      await _bookService.Update(code, book);

      return Ok();
    }
  }
}