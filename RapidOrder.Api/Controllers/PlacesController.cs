using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Entities;
using RapidOrder.Infrastructure;

namespace RapidOrder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacesController : ControllerBase
    {
        private readonly RapidOrderDbContext _db;
        public PlacesController(RapidOrderDbContext db) { _db = db; }

        [HttpGet]
        public Task<List<Place>> GetAll() =>
            _db.Places
                .Include(p => p.PlaceGroup)
                .Include(p => p.AssignedUser)
                .OrderBy(p => p.Number)
                .ToListAsync();

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Place p)
        {
            _db.Places.Add(p);
            await _db.SaveChangesAsync();
            return Ok(p);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Place p)
        {
            var existing = await _db.Places.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Number = p.Number;
            existing.Description = p.Description;
            existing.PlaceGroupId = p.PlaceGroupId;
            existing.AssignedUserId = p.AssignedUserId;
            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.Places.FindAsync(id);
            if (existing == null) return NotFound();
            _db.Places.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
