using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Employee.Domain;
using HrmsApi.Attributes;

namespace HrmsApi.Modules.Employee.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationsController : ControllerBase
    {
        private readonly HrmsDbContext _context;

        public DesignationsController(HrmsDbContext context)
        {
            _context = context;
        }

        // GET: api/Designations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Designation>>> GetDesignations()
        {
            return await _context.Designations.ToListAsync();
        }

        // GET: api/Designations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Designation>> GetDesignation(Guid id)
        {
            var designation = await _context.Designations.FindAsync(id);

            if (designation == null)
            {
                return NotFound();
            }

            return designation;
        }

        // POST: api/Designations
        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<Designation>> CreateDesignation(Designation designation)
        {
            _context.Designations.Add(designation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDesignation), new { id = designation.Id }, designation);
        }

        // PUT: api/Designations/5
        [HttpPut("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> UpdateDesignation(Guid id, Designation designation)
        {
            if (id != designation.Id)
            {
                return BadRequest();
            }

            _context.Entry(designation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DesignationExists(id))
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

        // DELETE: api/Designations/5
        [HttpDelete("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteDesignation(Guid id)
        {
            var designation = await _context.Designations.FindAsync(id);
            if (designation == null)
            {
                return NotFound();
            }

            _context.Designations.Remove(designation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DesignationExists(Guid id)
        {
            return _context.Designations.Any(e => e.Id == id);
        }
    }
}
