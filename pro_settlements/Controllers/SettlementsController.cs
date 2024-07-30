using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pro_settlements.Context;
using pro_settlements.Models;

namespace pro_settlements.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettlementsController : ControllerBase
    {
        private readonly DataContext _context;

        public SettlementsController(DataContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<IActionResult> GetSettlements(
    [FromQuery] string search = "",
    [FromQuery] int page = 1)
        {


            // Define page size
            int pageSize = 5;

            // Query settlements
            var query = _context.Settlements.AsQueryable();

            // Filter by search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.SettlementName.Contains(search));
            }


            // Pagination
            var settlements = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(settlements);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Settlement>> GetSettlement(int id)
        {
            var settlement = await _context.Settlements.FindAsync(id);

            if (settlement == null)
            {
                return NotFound();
            }

            return settlement;
        }

        [HttpPost]
        public async Task<IActionResult> PostSettlement([FromBody] string settlementName)
        {
            if (string.IsNullOrEmpty(settlementName))
            {
                return BadRequest("Settlement name is required.");
            }

            var existingSettlement = await _context.Settlements
                .FirstOrDefaultAsync(s => s.SettlementName == settlementName);

            if (existingSettlement != null)
            {
                return Conflict("A settlement with this name already exists.");
            }

            var settlement = new Settlement
            {
                SettlementName = settlementName
            };

            _context.Settlements.Add(settlement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSettlement), new { id = settlement.Id }, settlement);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSettlement(int id, [FromBody] string newName)
        {
            // חפש את האובייקט לפי ה-ID
            var settlement = await _context.Settlements.FindAsync(id);

            if (settlement == null)
            {
                return NotFound();
            }
            bool nameExists = await _context.Settlements
        .AnyAsync(s => s.SettlementName == newName && s.Id != id);

            if (nameExists)
            {
                // אם השם כבר קיים, החזר שגיאה מותאמת
                return Conflict("המושב כבר קיים במערכת.");
            }

            // עדכן את השם של האובייקט
            settlement.SettlementName = newName;

            // ציין שהאובייקט השתנה
            _context.Entry(settlement).State = EntityState.Modified;

            try
            {
                // שמור את השינויים ב-DB
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // אם האובייקט לא נמצא, החזר שגיאה
                if (!SettlementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSettlement(int id)
        {
            var settlement = await _context.Settlements.FindAsync(id);
            if (settlement == null)
            {
                return NotFound();
            }

            _context.Settlements.Remove(settlement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SettlementExists(int id)
        {
            return _context.Settlements.Any(e => e.Id == id);
        }
    }

}


