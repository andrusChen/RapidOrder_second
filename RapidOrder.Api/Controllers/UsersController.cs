using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Entities;
using RapidOrder.Infrastructure;

namespace RapidOrder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly RapidOrderDbContext _db;
        public UsersController(RapidOrderDbContext db) { _db = db; }

        [HttpGet]
        public Task<List<User>> GetAll() => _db.Users.OrderBy(u => u.DisplayName).ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(long id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();
            return u;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User u)
        {
            _db.Users.Add(u);
            await _db.SaveChangesAsync();
            return Ok(u);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] User u)
        {
            var existing = await _db.Users.FindAsync(id);
            if (existing == null) return NotFound();
            existing.DisplayName = u.DisplayName;
            existing.Role = u.Role;
            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();
            _db.Users.Remove(u);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}


