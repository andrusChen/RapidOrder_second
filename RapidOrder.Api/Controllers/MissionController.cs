using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidOrder.Infrastructure;
using RapidOrder.Core.Entities;
using Microsoft.Data.Sqlite;
using RapidOrder.Core.Enums;
using RapidOrder.Api.Services;

namespace RapidOrder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MissionsController : ControllerBase
    {
        private readonly RapidOrderDbContext _db;
        private readonly MissionAppService _missionService;

        public MissionsController(RapidOrderDbContext db, MissionAppService missionService)
        {
            _db = db;
            _missionService = missionService;
        }

        // GET: api/missions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mission>>> GetMissions()
        {
            try
            {
                return await _db.Missions
                    .OrderByDescending(m => m.StartedAt)
                    .ToListAsync();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 1) // "no such table"
            {
                return Ok(new List<Mission>()); // return empty list safely
            }
        }

        [HttpPost("{id}/acknowledge")]
        public async Task<IActionResult> Acknowledge(long id)
        {
            var updated = await _missionService.UpdateMissionAsync(id, MissionStatus.ACKNOWLEDGED, null);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        public record AssignReq(long UserId);
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> Assign(long id, [FromBody] AssignReq req)
        {
            var updated = await _missionService.UpdateMissionAsync(id, MissionStatus.ACKNOWLEDGED, req.UserId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpPost("{id}/finish")]
        public async Task<IActionResult> Finish(long id)
        {
            var updated = await _missionService.UpdateMissionAsync(id, MissionStatus.FINISHED, null);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(long id)
        {
            var updated = await _missionService.UpdateMissionAsync(id, MissionStatus.CANCELED, null);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // GET: api/missions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mission>> GetMission(int id)
        {
            var mission = await _db.Missions
                .Include(m => m.Place)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mission == null)
                return NotFound();

            return mission;
        }

        // DELETE: api/missions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMission(int id)
        {
            var mission = await _db.Missions.FindAsync(id);
            if (mission == null)
                return NotFound();

            _db.Missions.Remove(mission);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
