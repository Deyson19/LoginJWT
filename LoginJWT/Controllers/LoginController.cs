using LoginJWT.Helpers;
using LoginJWT.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
                    var user = await Authenticate(login);
                    if (user != null)
                    {
                         var token = GenerateToken(user);
                         return Ok(new
                         {
                              email = login.Email,
                              token,
                         });
                    }
                    return Unauthorized("Los datos ingresados no son correctos");
               }
               return BadRequest("No hay coincidencias");
          }
          [Authorize]
          [HttpGet("check-token")]
          public async Task<IActionResult> CheckToken()
          {
               var user = HttpContext.User;
               var token = await HttpContext.GetTokenAsync("access_token");
               var idUser = user.FindFirstValue(ClaimTypes.NameIdentifier);
               var usuarioActual = await GetSingleUser(Guid.Parse(idUser!));
               var isValidToken = Validate(token!);
               if (isValidToken)
               {
                    var x = GenerateToken(usuarioActual!);
                    token = x.ToString();
               }

               return Ok(new
               {
                    email = usuarioActual!.EmailAddress,
                    token = token
               });
          }
          private bool Validate(string token)
          {
               if (string.IsNullOrWhiteSpace(token)) { return false; }
               var secretkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));

               var audience = _config["Jwt:Audience"];
               var issuer = _config["Jwt:Issuer"];
               var tokenHandler = new JwtSecurityTokenHandler();
               try
               {
                    var validationParameters = new TokenValidationParameters
                    {
                         ValidateAudience = true,
                         ValidateIssuer = true,
                         ValidateLifetime = true,
                         ValidIssuer = issuer,
                         ValidAudience = audience,
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = secretkey,
                         ClockSkew = TimeSpan.Zero
                    };
                    tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                    return true;
               }
               catch (SecurityTokenException ex)
               {
                    return false;
               }
          }

          private async Task<User?> GetSingleUser(Guid idUsuario)
          {
               return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == idUsuario);
          }

          private async Task<User> Authenticate(Login login)
          {
               login.Password = HashPassword.GenerateHash(login.Password);
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
               var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

               //create claims
               var claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier,userDTO.Id.ToString()!),
                new(ClaimTypes.Email,userDTO.EmailAddress),
                new(ClaimTypes.GivenName,userDTO.FirstName),
                new(ClaimTypes.Surname,userDTO.LastName),
                new(ClaimTypes.Role,userDTO.Rol),
            };

               //create token security
               var token = new JwtSecurityToken(
                   _config["Jwt:Issuer"],
                   _config["Jwt:Audience"],
                   claims,
                   expires: DateTime.Now.AddMinutes(30),
                   signingCredentials: credentials
               );

               return new JwtSecurityTokenHandler().WriteToken(token);
          }
     }
}