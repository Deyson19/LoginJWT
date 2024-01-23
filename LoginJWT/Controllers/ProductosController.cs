using LoginJWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LoginJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly LoginDbContext _dbContext;

        public ProductosController(LoginDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Product>> Get() => await _dbContext.Products.ToListAsync();

        // POST api/<ProductosController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product value)
        {
            if (ModelState.IsValid)
            {
                var addProduct = await _dbContext.Products.AddAsync(value);
                await _dbContext.SaveChangesAsync();
                return Ok(addProduct.Context.ContextId);
            }
            return BadRequest();
        }

        // DELETE api/<ProductosController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product != null)
            {
                _dbContext.Remove(product);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
    }
}
