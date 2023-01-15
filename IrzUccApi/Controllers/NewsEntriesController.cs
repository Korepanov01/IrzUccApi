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
    public class NewsEntriesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public NewsEntriesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/NewsEntries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsEntry>>> GetNewsEntries()
        {
            return await _context.NewsEntries.ToListAsync();
        }

        // GET: api/NewsEntries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsEntry>> GetNewsEntry(int id)
        {
            var newsEntry = await _context.NewsEntries.FindAsync(id);

            if (newsEntry == null)
            {
                return NotFound();
            }

            return newsEntry;
        }

        // PUT: api/NewsEntries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNewsEntry(int id, NewsEntry newsEntry)
        {
            if (id != newsEntry.Id)
            {
                return BadRequest();
            }

            _context.Entry(newsEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsEntryExists(id))
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

        // POST: api/NewsEntries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NewsEntry>> PostNewsEntry(NewsEntry newsEntry)
        {
            _context.NewsEntries.Add(newsEntry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNewsEntry", new { id = newsEntry.Id }, newsEntry);
        }

        // DELETE: api/NewsEntries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsEntry(int id)
        {
            var newsEntry = await _context.NewsEntries.FindAsync(id);
            if (newsEntry == null)
            {
                return NotFound();
            }

            _context.NewsEntries.Remove(newsEntry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NewsEntryExists(int id)
        {
            return _context.NewsEntries.Any(e => e.Id == id);
        }
    }
}
