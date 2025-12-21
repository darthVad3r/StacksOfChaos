using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SOCApi.Models;
using SOCApi.Services.Book;
using Microsoft.AspNetCore.Authorization;

namespace SOCApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IBookService _bookService;

        public BookController(UserManager<User> userManager, IBookService bookService)
        {
            _userManager = userManager;
            _bookService = bookService;
        }

        // Add action methods here
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();

            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdBook = await _bookService.CreateBookAsync(book);
            if (createdBook == null) 
            {
                return StatusCode(500, "Failed to create book");
            }
            
            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedBook = await _bookService.UpdateBookAsync(id, book);
            if (updatedBook == null) return NotFound();

            return Ok(updatedBook);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}