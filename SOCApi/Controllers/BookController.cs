using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SOCApi.Models;
using SOCApi.Services.Book;
using SOCApi.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace SOCApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;

        public BookController(
            UserManager<User> userManager, 
            IBookService bookService,
            ILogger<BookController> logger)
        {
            _userManager = userManager;
            _bookService = bookService;
            _logger = logger;
        }

        /// <summary>
        /// Get all books for the authenticated user.
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(List<Book>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        /// <summary>
        /// Get a book by ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            
            if (book == null)
            {
                throw new NotFoundException("Book", id);
            }

            return Ok(book);
        }

        /// <summary>
        /// Create a new book.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                    );
                
                throw new ValidationException(errors);
            }

            var createdBook = await _bookService.CreateBookAsync(book);
            
            if (createdBook == null)
            {
                throw new BusinessLogicException("Failed to create book. Please check the provided data.");
            }

            _logger.LogInformation("Book created with ID: {BookId}", createdBook.Id);
            
            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
        }

        /// <summary>
        /// Update an existing book.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                    );
                
                throw new ValidationException(errors);
            }

            book.Id = id;

            var updatedBook = await _bookService.UpdateBookAsync(book);
            
            if (updatedBook == null)
            {
                throw new NotFoundException("Book", id);
            }

            _logger.LogInformation("Book updated with ID: {BookId}", id);

            return Ok(updatedBook);
        }

        /// <summary>
        /// Delete a book by ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            
            if (!result)
            {
                throw new NotFoundException("Book", id);
            }

            _logger.LogInformation("Book deleted with ID: {BookId}", id);

            return NoContent();
        }
    }
}