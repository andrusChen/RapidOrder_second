using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;
using RapidOrder.Infrastructure;

namespace RapidOrder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CallButtonsController : ControllerBase
    {
        private readonly RapidOrderDbContext _db;
        public CallButtonsController(RapidOrderDbContext db) { _db = db; }

        [HttpGet]
        public Task<List<CallButton>> GetAll() =>
            _db.CallButtons.Include(c => c.Place).ToListAsync();

        public record MapReq(int ButtonNumber, MissionType MissionType);

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CallButton cb)
        {
            _db.CallButtons.Add(cb);
            await _db.SaveChangesAsync();
            return Ok(cb);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CallButton cb)
        {
            var existing = await _db.CallButtons.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Label = cb.Label;
            existing.PlaceId = cb.PlaceId;
            existing.ButtonId = cb.ButtonId;
            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpPost("{id}/assign-place/{placeId}")]
        public async Task<IActionResult> AssignPlace(int id, int placeId)
        {
            var cb = await _db.CallButtons.FindAsync(id);
            if (cb == null) return NotFound();
            var place = await _db.Places.FindAsync(placeId);
            if (place == null) return NotFound();
            cb.PlaceId = placeId;
            await _db.SaveChangesAsync();
            return Ok(cb);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cb = await _db.CallButtons.FindAsync(id);
            if (cb == null) return NotFound();
            _db.CallButtons.Remove(cb);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
