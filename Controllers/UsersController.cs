using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TicketManagementAPI.Models;

namespace TicketManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context) => _context = context;

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound("No users found.");
            }
            try
            {
                var users = await _context.Users.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving data from the database: {ex.Message}");
            }
        }
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving data from the database: {ex.Message}");
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginData)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginData.Email && u.PasswordHash == loginData.Password);

            if (user == null || user.IsActive == false)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(user);
        }
 
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            // Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
                return BadRequest("Email already registered");
            if(model.ProfilePictureFile != null)
            {
                using var memoryStream = new MemoryStream();
                await model.ProfilePictureFile.CopyToAsync(memoryStream);
                var pictureBytes = memoryStream.ToArray();

                var newUser = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = model.Password, // In a real application, hash the password before saving
                    ProfilePicture = pictureBytes,
                };
                _context.Users.Add(newUser);
            }
            else
            {
                model.ProfilePicture = null; // Handle case where no profile picture is uploaded
            }
            // Hash the password
            //string passwordHash = ComputeSha256Hash(model.Password);

            // Create user entity
            

            // Save to database
            //_context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful" });
        }

    }
}
