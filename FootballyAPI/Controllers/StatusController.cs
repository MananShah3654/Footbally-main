using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballyAPI.Data;
using FootballyAPI.Models;

namespace FootballyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly FootballyDbContext _context;
        private readonly ILogger<StatusController> _logger;

        public StatusController(FootballyDbContext context, ILogger<StatusController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusCheck>>> GetStatusChecks()
        {
            try
            {
                var statusChecks = await _context.StatusChecks
                    .OrderByDescending(sc => sc.Timestamp)
                    .Take(100)
                    .ToListAsync();

                return Ok(statusChecks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving status checks");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<StatusCheck>> CreateStatusCheck([FromBody] CreateStatusCheckDto input)
        {
            try
            {
                var statusCheck = new StatusCheck
                {
                    ClientName = input.ClientName,
                    Message = input.Message ?? "Status check performed",
                    Status = "Active"
                };

                _context.StatusChecks.Add(statusCheck);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetStatusCheck), new { id = statusCheck.Id }, statusCheck);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating status check");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StatusCheck>> GetStatusCheck(string id)
        {
            try
            {
                var statusCheck = await _context.StatusChecks.FindAsync(id);
                if (statusCheck == null)
                    return NotFound($"Status check with ID {id} not found");

                return Ok(statusCheck);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving status check {StatusCheckId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { 
                Status = "Healthy", 
                Timestamp = DateTime.UtcNow,
                Service = "Footbally API",
                Version = "1.0.0"
            });
        }
    }

    public class CreateStatusCheckDto
    {
        public string ClientName { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}