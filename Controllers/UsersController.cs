using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketManagementAPI.Models;

namespace TicketManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public UsersController(ApplicationDbContext context, IConfiguration conconfig) {

            _context = context;
            _config = conconfig;
        }

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
            //var user = await _context.Users
            //    .FirstOrDefaultAsync(u => u.Email == loginData.Email);
            //string base64Image = user.ProfilePicture != null ? Convert.ToBase64String(user.ProfilePicture) : null;
            //if (user != null && user.IsActive == true && BCrypt.Net.BCrypt.Verify(loginData.Password, user.PasswordHash))
            //{

            //    return Ok(new
            //    {
            //        user.UserId,
            //        user.FullName,
            //        user.Email,
            //        ProfilePictureBase64 = base64Image
            //    });
            //}
            //else
            //{
            //    return Ok(new
            //    {
            //        success = false,
            //        message = "Invalid credentials"
            //    });
            //}
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginData.Email);

            if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(loginData.Password, user.PasswordHash))
            {
                return Ok(new { success = false, message = "Invalid credentials" });
            }

            string base64Image = user.ProfilePicture != null ? Convert.ToBase64String(user.ProfilePicture) : null;

            // ✅ Generate JWT token
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                success = true,
                token,
                user = new
                {
                    user.UserId,
                    user.FullName,
                    user.Email,
                    ProfilePictureBase64 = base64Image
                }
            });
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
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                var newUser = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = hashedPassword, // In a real application, hash the password before saving
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

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, "User") // Optional: Add role support
    };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
