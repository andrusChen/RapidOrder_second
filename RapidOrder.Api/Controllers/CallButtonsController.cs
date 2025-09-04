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
            _db.CallButtons.Include(c => c.Place).Include(c => c.ActionMaps).ToListAsync();

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
            var cb = await _db.CallButtons.Include(c => c.ActionMaps).FirstOrDefaultAsync(c => c.Id == id);
            if (cb == null) return NotFound();

            var existing = cb.ActionMaps.FirstOrDefault(m => m.ButtonNumber == req.ButtonNumber);
            if (existing == null)
                cb.ActionMaps.Add(new CallButtonActionMap { ButtonNumber = req.ButtonNumber, MissionType = req.MissionType });
            else
                existing.MissionType = req.MissionType;

            await _db.SaveChangesAsync();
            return Ok(cb);
        }
    }
}
