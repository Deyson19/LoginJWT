
using LoginJWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly LoginDbContext _dbContext;

        public UserController(LoginDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAll()
        {
            var result = await _dbContext.Users.ToListAsync();
            return Ok(result);
        }
        [HttpGet("GetAll")]
        //[Authorize(Roles = "Client")]

        public async Task<IActionResult> GetAllAny()
        {
            var result = await _dbContext.Users.ToListAsync();
            return Ok(new { isSuccess = result.Any(), total = result.Count(), data = result });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                var response = await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "user created" });
            }
            return BadRequest();
        }

    }
}