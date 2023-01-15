using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IrzUccApi;
using IrzUccApi.Models;

namespace IrzUccApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionHistoricalRecordsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PositionHistoricalRecordsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/PositionHistoricalRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PositionHistoricalRecord>>> GetPositionHistoricalRecords()
        {
            return await _context.PositionHistoricalRecords.ToListAsync();
        }

        // GET: api/PositionHistoricalRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PositionHistoricalRecord>> GetPositionHistoricalRecord(int id)
        {
            var positionHistoricalRecord = await _context.PositionHistoricalRecords.FindAsync(id);

            if (positionHistoricalRecord == null)
            {
                return NotFound();
            }

            return positionHistoricalRecord;
        }

        // PUT: api/PositionHistoricalRecords/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPositionHistoricalRecord(int id, PositionHistoricalRecord positionHistoricalRecord)
        {
            if (id != positionHistoricalRecord.Id)
            {
                return BadRequest();
            }

            _context.Entry(positionHistoricalRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionHistoricalRecordExists(id))
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

        // POST: api/PositionHistoricalRecords
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PositionHistoricalRecord>> PostPositionHistoricalRecord(PositionHistoricalRecord positionHistoricalRecord)
        {
            _context.PositionHistoricalRecords.Add(positionHistoricalRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPositionHistoricalRecord", new { id = positionHistoricalRecord.Id }, positionHistoricalRecord);
        }

        // DELETE: api/PositionHistoricalRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePositionHistoricalRecord(int id)
        {
            var positionHistoricalRecord = await _context.PositionHistoricalRecords.FindAsync(id);
            if (positionHistoricalRecord == null)
            {
                return NotFound();
            }

            _context.PositionHistoricalRecords.Remove(positionHistoricalRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PositionHistoricalRecordExists(int id)
        {
            return _context.PositionHistoricalRecords.Any(e => e.Id == id);
        }
    }
}
