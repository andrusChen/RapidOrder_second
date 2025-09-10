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

        
    }
}
