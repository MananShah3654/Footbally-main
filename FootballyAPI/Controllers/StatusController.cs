using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballyAPI.Data;
using FootballyAPI.Models;

namespace FootballyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly FootballyDbContext _context;

        public StatusController(FootballyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetStatus()
        {
            try
            {
                // Test database connection
                var dbStatus = await _context.Database.CanConnectAsync();
                
                var response = new
                {
                    Status = "OK",
                    Timestamp = DateTime.UtcNow,
                    Database = dbStatus ? "Connected" : "Disconnected",
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    Status = "Error",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message,
                    Version = "1.0.0"
                };

                return StatusCode(500, response);
            }
        }

        [HttpPost("check")]
        public async Task<ActionResult<StatusCheck>> CreateStatusCheck([FromBody] StatusCheckCreateDto input)
        {
            var statusCheck = new StatusCheck
            {
                ClientName = input.ClientName
            };

            _context.StatusChecks.Add(statusCheck);
            await _context.SaveChangesAsync();

            return Ok(statusCheck);
        }

        [HttpGet("checks")]
        public async Task<ActionResult<IEnumerable<StatusCheck>>> GetStatusChecks()
        {
            var statusChecks = await _context.StatusChecks
                .OrderByDescending(sc => sc.Timestamp)
                .Take(100)
                .ToListAsync();

            return Ok(statusChecks);
        }
    }

    public class StatusCheckCreateDto
    {
        public string ClientName { get; set; } = string.Empty;
    }
}