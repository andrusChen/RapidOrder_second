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
            _db.Places.Include(p => p.PlaceGroup).OrderBy(p => p.Number).ToListAsync();

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Place p)
        {
            _db.Places.Add(p);
            await _db.SaveChangesAsync();
            return Ok(p);
        }
    }
}
