using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Dashboard.Domain;
using HrmsApi.Modules.Dashboard.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HrmsApi.Modules.Dashboard.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotivationalQuoteController : ControllerBase
    {
        private readonly HrmsDbContext _context;
        private static readonly Random _random = new Random();

        public MotivationalQuoteController(HrmsDbContext context)
        {
            _context = context;
        }

        // GET: api/MotivationalQuote/Random
        [HttpGet("Random")]
        public async Task<ActionResult<MotivationalQuoteDTO>> GetRandomQuote()
        {
            try
            {
                var activeQuotes = await _context.MotivationalQuotes
                    .Where(q => q.IsActive)
                    .ToListAsync();

                if (!activeQuotes.Any())
                {
                    return NotFound("No motivational quotes available");
                }

                // Select a random quote
                var randomQuote = activeQuotes[_random.Next(activeQuotes.Count)];

                return new MotivationalQuoteDTO
                {
                    Id = randomQuote.Id,
                    QuoteText = randomQuote.QuoteText,
                    Author = randomQuote.Author,
                    Category = randomQuote.Category
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/MotivationalQuote
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MotivationalQuoteDTO>>> GetAllQuotes()
        {
            try
            {
                var quotes = await _context.MotivationalQuotes.ToListAsync();

                var result = quotes.Select(q => new MotivationalQuoteDTO
                {
                    Id = q.Id,
                    QuoteText = q.QuoteText,
                    Author = q.Author,
                    Category = q.Category,
                    IsActive = q.IsActive
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/MotivationalQuote
        [HttpPost]
        public async Task<ActionResult<MotivationalQuoteDTO>> CreateQuote([FromBody] CreateQuoteDTO quoteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var quote = new MotivationalQuote
                {
                    Id = Guid.NewGuid(),
                    QuoteText = quoteDto.QuoteText,
                    Author = quoteDto.Author,
                    Category = quoteDto.Category,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                _context.MotivationalQuotes.Add(quote);
                await _context.SaveChangesAsync();

                var result = new MotivationalQuoteDTO
                {
                    Id = quote.Id,
                    QuoteText = quote.QuoteText,
                    Author = quote.Author,
                    Category = quote.Category,
                    IsActive = quote.IsActive
                };

                return CreatedAtAction(nameof(GetAllQuotes), new { id = quote.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/MotivationalQuote/5/toggle
        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> ToggleQuoteStatus(Guid id)
        {
            try
            {
                var quote = await _context.MotivationalQuotes.FindAsync(id);
                if (quote == null)
                {
                    return NotFound();
                }

                quote.IsActive = !quote.IsActive;
                quote.UpdatedAt = DateTime.UtcNow;
                quote.UpdatedBy = "System";

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/MotivationalQuote/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(Guid id)
        {
            try
            {
                var quote = await _context.MotivationalQuotes.FindAsync(id);
                if (quote == null)
                {
                    return NotFound();
                }

                _context.MotivationalQuotes.Remove(quote);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
