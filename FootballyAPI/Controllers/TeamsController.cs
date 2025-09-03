using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballyAPI.Data;
using FootballyAPI.Models;
using FootballyAPI.DTOs;

namespace FootballyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly FootballyDbContext _context;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(FootballyDbContext context, ILogger<TeamsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams(
            [FromQuery] string? league = null,
            [FromQuery] string? country = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _context.Teams.AsQueryable();

                if (!string.IsNullOrEmpty(league))
                    query = query.Where(t => t.League.ToLower().Contains(league.ToLower()));

                if (!string.IsNullOrEmpty(country))
                    query = query.Where(t => t.Country.ToLower().Contains(country.ToLower()));

                var totalCount = await query.CountAsync();
                var teams = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TeamDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        ShortName = t.ShortName,
                        City = t.City,
                        Country = t.Country,
                        Stadium = t.Stadium,
                        Founded = t.Founded,
                        PrimaryColor = t.PrimaryColor,
                        SecondaryColor = t.SecondaryColor,
                        LogoUrl = t.LogoUrl,
                        Manager = t.Manager,
                        League = t.League,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt
                    })
                    .OrderBy(t => t.Name)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(teams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teams");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(string id)
        {
            try
            {
                var team = await _context.Teams
                    .Where(t => t.Id == id)
                    .Select(t => new TeamDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        ShortName = t.ShortName,
                        City = t.City,
                        Country = t.Country,
                        Stadium = t.Stadium,
                        Founded = t.Founded,
                        PrimaryColor = t.PrimaryColor,
                        SecondaryColor = t.SecondaryColor,
                        LogoUrl = t.LogoUrl,
                        Manager = t.Manager,
                        League = t.League,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (team == null)
                    return NotFound($"Team with ID {id} not found");

                return Ok(team);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving team {TeamId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TeamDto>> CreateTeam(CreateTeamDto createTeamDto)
        {
            try
            {
                var team = new Team
                {
                    Name = createTeamDto.Name,
                    ShortName = createTeamDto.ShortName,
                    City = createTeamDto.City,
                    Country = createTeamDto.Country,
                    Stadium = createTeamDto.Stadium,
                    Founded = createTeamDto.Founded,
                    PrimaryColor = createTeamDto.PrimaryColor,
                    SecondaryColor = createTeamDto.SecondaryColor,
                    LogoUrl = createTeamDto.LogoUrl,
                    Manager = createTeamDto.Manager,
                    League = createTeamDto.League
                };

                _context.Teams.Add(team);
                await _context.SaveChangesAsync();

                var teamDto = new TeamDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    ShortName = team.ShortName,
                    City = team.City,
                    Country = team.Country,
                    Stadium = team.Stadium,
                    Founded = team.Founded,
                    PrimaryColor = team.PrimaryColor,
                    SecondaryColor = team.SecondaryColor,
                    LogoUrl = team.LogoUrl,
                    Manager = team.Manager,
                    League = team.League,
                    CreatedAt = team.CreatedAt,
                    UpdatedAt = team.UpdatedAt
                };

                return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, teamDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/players")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetTeamPlayers(string id)
        {
            try
            {
                var teamExists = await _context.Teams.AnyAsync(t => t.Id == id);
                if (!teamExists)
                    return NotFound($"Team with ID {id} not found");

                var players = await _context.Players
                    .Where(p => p.TeamId == id)
                    .Select(p => new PlayerDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Position = p.Position,
                        Age = p.Age,
                        Nationality = p.Nationality,
                        TeamId = p.TeamId,
                        Height = p.Height,
                        Weight = p.Weight,
                        PreferredFoot = p.PreferredFoot,
                        JerseyNumber = p.JerseyNumber,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .OrderBy(p => p.JerseyNumber)
                    .ToListAsync();

                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving team players for {TeamId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(string id)
        {
            try
            {
                var team = await _context.Teams.FindAsync(id);
                if (team == null)
                    return NotFound($"Team with ID {id} not found");

                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting team {TeamId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}