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
    public class PlayersController : ControllerBase
    {
        private readonly FootballyDbContext _context;

        public PlayersController(FootballyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers([FromQuery] string? teamId = null)
        {
            var query = _context.Players.AsQueryable();

            if (!string.IsNullOrEmpty(teamId))
                query = query.Where(p => p.TeamId == teamId);

            var players = await query
                .Select(p => new PlayerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Position = p.Position,
                    Age = p.Age,
                    Nationality = p.Nationality,
                    TeamId = p.TeamId,
                    JerseyNumber = p.JerseyNumber,
                    Height = p.Height,
                    Weight = p.Weight,
                    PreferredFoot = p.PreferredFoot,
                    PhotoUrl = p.PhotoUrl
                })
                .ToListAsync();

            return Ok(players);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDetailDto>> GetPlayer(string id)
        {
            var player = await _context.Players
                .Include(p => p.Statistics)
                .Include(p => p.MatchPerformances)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
                return NotFound();

            var team = await _context.Teams.FindAsync(player.TeamId);

            var playerDetail = new PlayerDetailDto
            {
                Id = player.Id,
                Name = player.Name,
                Position = player.Position,
                Age = player.Age,
                Nationality = player.Nationality,
                TeamId = player.TeamId,
                TeamName = team?.Name ?? "",
                JerseyNumber = player.JerseyNumber,
                Height = player.Height,
                Weight = player.Weight,
                PreferredFoot = player.PreferredFoot,
                PhotoUrl = player.PhotoUrl,
                Statistics = player.Statistics.Select(s => new PlayerStatisticsDto
                {
                    Season = s.Season,
                    GamesPlayed = s.GamesPlayed,
                    Goals = s.Goals,
                    Assists = s.Assists,
                    AverageRating = s.AverageRating
                }).ToList(),
                RecentMatches = player.MatchPerformances.OrderByDescending(mp => mp.CreatedAt)
                    .Take(10)
                    .Select(mp => new MatchPerformanceDto
                    {
                        MatchId = mp.MatchId,
                        Goals = mp.Goals,
                        Assists = mp.Assists,
                        MinutesPlayed = mp.MinutesPlayed
                    }).ToList()
            };

            return Ok(playerDetail);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PlayerDto>> CreatePlayer([FromBody] CreatePlayerDto createPlayerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if team exists
            var teamExists = await _context.Teams.AnyAsync(t => t.Id == createPlayerDto.TeamId);
            if (!teamExists)
                return BadRequest("Team not found");

            var player = new Player
            {
                Name = createPlayerDto.Name,
                Position = createPlayerDto.Position,
                Age = createPlayerDto.Age,
                Nationality = createPlayerDto.Nationality,
                TeamId = createPlayerDto.TeamId,
                Height = createPlayerDto.Height,
                Weight = createPlayerDto.Weight,
                PreferredFoot = createPlayerDto.PreferredFoot,
                JerseyNumber = createPlayerDto.JerseyNumber,
                PhotoUrl = createPlayerDto.PhotoUrl
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            var playerDto = new PlayerDto
            {
                Id = player.Id,
                Name = player.Name,
                Position = player.Position,
                Age = player.Age,
                Nationality = player.Nationality,
                TeamId = player.TeamId,
                JerseyNumber = player.JerseyNumber,
                Height = player.Height,
                Weight = player.Weight,
                PreferredFoot = player.PreferredFoot,
                PhotoUrl = player.PhotoUrl
            };

            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, playerDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePlayer(string id, [FromBody] UpdatePlayerDto updatePlayerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var player = await _context.Players.FindAsync(id);
            if (player == null)
                return NotFound();

            // Check if team exists
            var teamExists = await _context.Teams.AnyAsync(t => t.Id == updatePlayerDto.TeamId);
            if (!teamExists)
                return BadRequest("Team not found");

            player.Name = updatePlayerDto.Name;
            player.Position = updatePlayerDto.Position;
            player.Age = updatePlayerDto.Age;
            player.Nationality = updatePlayerDto.Nationality;
            player.TeamId = updatePlayerDto.TeamId;
            player.Height = updatePlayerDto.Height;
            player.Weight = updatePlayerDto.Weight;
            player.PreferredFoot = updatePlayerDto.PreferredFoot;
            player.JerseyNumber = updatePlayerDto.JerseyNumber;
            player.PhotoUrl = updatePlayerDto.PhotoUrl;
            player.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlayer(string id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
                return NotFound();

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}