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

        [HttpPost("{id:int}/map")]
        public async Task<IActionResult> MapButton(int id, [FromBody] MapReq req)
        {
            var cb = await _db.CallButtons.FirstOrDefaultAsync(c => c.Id == id);
            if (cb == null) return NotFound();

            // Instead of persisting to ActionMaps table,
            // you must decide how to store mapping (e.g. config file, enum switch, etc.)
            // Example: just return fake mapping
            var map = new { cb.Id, req.ButtonNumber, req.MissionType };

            return Ok(map);
        }
    }
}
