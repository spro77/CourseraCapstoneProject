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
    public class ProjectsController : ControllerBase
    {
        private readonly SkillSnapContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProjectsController> _logger;
        private const string ProjectsCacheKey = "projects_list";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        public ProjectsController(
            SkillSnapContext context,
            IMemoryCache cache,
            ILogger<ProjectsController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        // GET: api/projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var stopwatch = Stopwatch.StartNew();

            if (!_cache.TryGetValue(ProjectsCacheKey, out List<Project>? projects))
            {
                _logger.LogInformation("Cache miss for projects list - fetching from database");

                projects = await _context.Projects
                    .AsNoTracking()
                    .ToListAsync();

                _cache.Set(ProjectsCacheKey, projects, CacheDuration);

                stopwatch.Stop();
                _logger.LogInformation("Projects loaded from database in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }
            else
            {
                stopwatch.Stop();
                _logger.LogInformation("Cache hit for projects list - returned in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }

            return Ok(projects);
        }

        // GET: api/projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // POST: api/projects
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Invalidate cache after modification
            _cache.Remove(ProjectsCacheKey);
            _logger.LogInformation("Cache invalidated after creating project {ProjectId}", project.Id);

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        // PUT: api/projects/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Invalidate cache after modification
                _cache.Remove(ProjectsCacheKey);
                _logger.LogInformation("Cache invalidated after updating project {ProjectId}", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // DELETE: api/projects/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            // Invalidate cache after deletion
            _cache.Remove(ProjectsCacheKey);
            _logger.LogInformation("Cache invalidated after deleting project {ProjectId}", id);

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
