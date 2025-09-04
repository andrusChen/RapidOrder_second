using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidOrder.Infrastructure;
using RapidOrder.Core.Entities;

namespace RapidOrder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MissionsController : ControllerBase
    {
        private readonly RapidOrderDbContext _db;

        public MissionsController(RapidOrderDbContext db)
        {
            _db = db;
        }

        // GET: api/missions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mission>>> GetMissions()
        {
            return await _db.Missions
                .Include(m => m.Place)
                .OrderByDescending(m => m.StartedAt)
                .ToListAsync();
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
