using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;
using RapidOrder.Infrastructure;

namespace RapidOrder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionMapsController : ControllerBase
    {
        private readonly RapidOrderDbContext _db;
        public ActionMapsController(RapidOrderDbContext db) { _db = db; }

        [HttpGet]
        public Task<List<ActionMap>> GetAll() =>
            _db.ActionMaps.OrderBy(a => a.DeviceCode).ThenBy(a => a.ButtonNumber).ToListAsync();

        public record UpsertReq(string DeviceCode, int ButtonNumber, MissionType MissionType);

        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert([FromBody] UpsertReq req)
        {
            var existing = await _db.ActionMaps.FirstOrDefaultAsync(a => a.DeviceCode == req.DeviceCode && a.ButtonNumber == req.ButtonNumber);
            if (existing == null)
            {
                existing = new ActionMap
                {
                    DeviceCode = req.DeviceCode,
                    ButtonNumber = req.ButtonNumber,
                    MissionType = req.MissionType
                };
                _db.ActionMaps.Add(existing);
            }
            else
            {
                existing.MissionType = req.MissionType;
            }

            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var am = await _db.ActionMaps.FindAsync(id);
            if (am == null) return NotFound();
            _db.ActionMaps.Remove(am);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}


