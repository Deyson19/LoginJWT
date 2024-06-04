using LoginJWT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly LoginDbContext _dbContext;

        public LoginController(IConfiguration config, LoginDbContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (ModelState.IsValid)
            {
                var user = await Autenticate(login);
                if (user != null)
                {
                    var token = GenerateToken(user);
                    return Ok(new
                    {
                        email = login.Email,
                        token = token,
                    });
                }
                return Unauthorized("Los datos ingresados no son correctos");
            }
            return BadRequest("No hay coincidencias");
        }

        private async Task<User> Autenticate(Login login)
        {
            var getUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.EmailAddress == login.Email.ToLower() && x.Password == login.Password);
            if (getUser != null)
            {
                return getUser;
            }
            return null!;
        }
        private string GenerateToken(User userDTO)
        {
            var secretkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));
            var credemtials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

            //create claims
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,userDTO.Username),
                new Claim(ClaimTypes.Email,userDTO.EmailAddress),
                new Claim(ClaimTypes.GivenName,userDTO.FirstName),
                new Claim(ClaimTypes.Surname,userDTO.LastName),
                new Claim(ClaimTypes.Role,userDTO.Rol),
            };

            //create token security
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credemtials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}