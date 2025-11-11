using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Api.Models;
using System.Diagnostics;

namespace SkillSnap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly SkillSnapContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SkillsController> _logger;
        private const string SkillsCacheKey = "skills_list";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        public SkillsController(
            SkillSnapContext context,
            IMemoryCache cache,
            ILogger<SkillsController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        // GET: api/skills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            var stopwatch = Stopwatch.StartNew();

            if (!_cache.TryGetValue(SkillsCacheKey, out List<Skill>? skills))
            {
                _logger.LogInformation("Cache miss for skills list - fetching from database");

                skills = await _context.Skills
                    .AsNoTracking()
                    .ToListAsync();

                _cache.Set(SkillsCacheKey, skills, CacheDuration);

                stopwatch.Stop();
                _logger.LogInformation("Skills loaded from database in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }
            else
            {
                stopwatch.Stop();
                _logger.LogInformation("Cache hit for skills list - returned in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }

            return Ok(skills);
        }

        // GET: api/skills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkill(int id)
        {
            var skill = await _context.Skills
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
            {
                return NotFound();
            }

            return skill;
        }

        // POST: api/skills
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Skill>> PostSkill(Skill skill)
        {
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            // Invalidate cache after modification
            _cache.Remove(SkillsCacheKey);
            _logger.LogInformation("Cache invalidated after creating skill {SkillId}", skill.Id);

            return CreatedAtAction(nameof(GetSkill), new { id = skill.Id }, skill);
        }

        // PUT: api/skills/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutSkill(int id, Skill skill)
        {
            if (id != skill.Id)
            {
                return BadRequest();
            }

            _context.Entry(skill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Invalidate cache after modification
                _cache.Remove(SkillsCacheKey);
                _logger.LogInformation("Cache invalidated after updating skill {SkillId}", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillExists(id))
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

        // DELETE: api/skills/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound();
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            // Invalidate cache after deletion
            _cache.Remove(SkillsCacheKey);
            _logger.LogInformation("Cache invalidated after deleting skill {SkillId}", id);

            return NoContent();
        }

        private bool SkillExists(int id)
        {
            return _context.Skills.Any(e => e.Id == id);
        }
    }
}
