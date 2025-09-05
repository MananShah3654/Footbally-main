using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballyAPI.Data;
using FootballyAPI.Models;
using FootballyAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace FootballyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly FootballyDbContext _context;

        public TeamsController(FootballyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams()
        {
            var teams = await _context.Teams
                .Include(t => t.Players)
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
                    Manager = t.Manager,
                    League = t.League,
                    LogoUrl = t.LogoUrl,
                    PlayerCount = t.Players.Count
                })
                .ToListAsync();

            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDetailDto>> GetTeam(string id)
        {
            var team = await _context.Teams
                .Include(t => t.Players)
                .Include(t => t.TournamentTeams)
                    .ThenInclude(tt => tt.Tournament)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return NotFound();

            var teamDetail = new TeamDetailDto
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
                Manager = team.Manager,
                League = team.League,
                LogoUrl = team.LogoUrl,
                Players = team.Players.Select(p => new PlayerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Position = p.Position,
                    Age = p.Age,
                    Nationality = p.Nationality,
                    JerseyNumber = p.JerseyNumber,
                    Height = p.Height,
                    Weight = p.Weight,
                    PreferredFoot = p.PreferredFoot,
                    PhotoUrl = p.PhotoUrl
                }).ToList(),
                Tournaments = team.TournamentTeams.Select(tt => new TournamentDto
                {
                    Id = tt.Tournament.Id,
                    Name = tt.Tournament.Name,
                    Format = tt.Tournament.Format,
                    Status = tt.Tournament.Status,
                    StartDate = tt.Tournament.StartDate,
                    EndDate = tt.Tournament.EndDate
                }).ToList()
            };

            return Ok(teamDetail);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] CreateTeamDto createTeamDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
                Manager = createTeamDto.Manager,
                League = createTeamDto.League,
                LogoUrl = createTeamDto.LogoUrl
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
                Manager = team.Manager,
                League = team.League,
                LogoUrl = team.LogoUrl,
                PlayerCount = 0
            };

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, teamDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTeam(string id, [FromBody] UpdateTeamDto updateTeamDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return NotFound();

            team.Name = updateTeamDto.Name;
            team.ShortName = updateTeamDto.ShortName;
            team.City = updateTeamDto.City;
            team.Country = updateTeamDto.Country;
            team.Stadium = updateTeamDto.Stadium;
            team.Founded = updateTeamDto.Founded;
            team.PrimaryColor = updateTeamDto.PrimaryColor;
            team.SecondaryColor = updateTeamDto.SecondaryColor;
            team.Manager = updateTeamDto.Manager;
            team.League = updateTeamDto.League;
            team.LogoUrl = updateTeamDto.LogoUrl;
            team.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTeam(string id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return NotFound();

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}