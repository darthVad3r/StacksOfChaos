using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOCApi.Data;
using SOCApi.Models;
using SOCApi.Dto;

namespace SOCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SOCApiContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(SOCApiContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            _logger.LogInformation("Retrieving all users.");
            return await _context.Users.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with id {Id} not found.", id);
                return NotFound(new { error = "User not found." });
            }
            return user;
        }

        // POST: api/User/register
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> RegisterNewUserAsync(UserCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid user creation model state.");
                return BadRequest(ModelState);
            }
            var user = new User { Username = dto.Username, Email = dto.Email };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User created with id {Id}.", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, UserUpdateDto dto)
        {
            if (id != dto.Id)
            {
                _logger.LogWarning("User update id mismatch: {Id} vs {DtoId}", id, dto.Id);
                return BadRequest(new { error = "Id mismatch." });
            }
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid user update model state.");
                return BadRequest(ModelState);
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with id {Id} not found for update.", id);
                return NotFound(new { error = "User not found." });
            }
            user.Username = dto.Username;
            user.Email = dto.Email;
            // Update other fields as needed
            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    _logger.LogWarning("User with id {Id} not found during concurrency exception.", id);
                    return NotFound(new { error = "User not found." });
                }
                else
                {
                    throw;
                }
            }
            _logger.LogInformation("User with id {Id} updated.", id);
            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with id {Id} not found for deletion.", id);
                return NotFound(new { error = "User not found." });
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User with id {Id} deleted.", id);
            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
