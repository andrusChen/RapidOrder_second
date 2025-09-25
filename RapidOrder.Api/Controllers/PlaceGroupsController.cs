using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Entities;
using RapidOrder.Infrastructure;

namespace RapidOrder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceGroupsController : ControllerBase
    {
        private readonly RapidOrderDbContext _db;
        public PlaceGroupsController(RapidOrderDbContext db) { _db = db; }

        [HttpGet]
        public Task<List<PlaceGroup>> GetAll() =>
            _db.PlaceGroups
                .Include(g => g.Places)
                .OrderBy(g => g.Name)
                .ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<PlaceGroup>> GetById(int id)
        {
            var group = await _db.PlaceGroups.Include(g => g.Places).FirstOrDefaultAsync(g => g.Id == id);
            if (group == null) return NotFound();
            return group;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlaceGroup g)
        {
            _db.PlaceGroups.Add(g);
            await _db.SaveChangesAsync();
            return Ok(g);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PlaceGroup g)
        {
            var existing = await _db.PlaceGroups.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = g.Name;
            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.PlaceGroups.FindAsync(id);
            if (existing == null) return NotFound();
            _db.PlaceGroups.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}


