using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballyAPI.Data;
using FootballyAPI.Models;
using FootballyAPI.DTOs;

namespace FootballyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly FootballyDbContext _context;
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(FootballyDbContext context, ILogger<PlayersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers(
            [FromQuery] string? teamId = null,
            [FromQuery] string? position = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _context.Players.AsQueryable();

                if (!string.IsNullOrEmpty(teamId))
                    query = query.Where(p => p.TeamId == teamId);

                if (!string.IsNullOrEmpty(position))
                    query = query.Where(p => p.Position.ToLower().Contains(position.ToLower()));

                var totalCount = await query.CountAsync();
                var players = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
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
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving players");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetPlayer(string id)
        {
            try
            {
                var player = await _context.Players
                    .Where(p => p.Id == id)
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
                    .FirstOrDefaultAsync();

                if (player == null)
                    return NotFound($"Player with ID {id} not found");

                return Ok(player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving player {PlayerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> CreatePlayer(CreatePlayerDto createPlayerDto)
        {
            try
            {
                // Check if team exists
                if (!string.IsNullOrEmpty(createPlayerDto.TeamId))
                {
                    var teamExists = await _context.Teams.AnyAsync(t => t.Id == createPlayerDto.TeamId);
                    if (!teamExists)
                        return BadRequest($"Team with ID {createPlayerDto.TeamId} not found");
                }

                // Check jersey number uniqueness within team
                if (!string.IsNullOrEmpty(createPlayerDto.TeamId))
                {
                    var jerseyExists = await _context.Players
                        .AnyAsync(p => p.TeamId == createPlayerDto.TeamId && p.JerseyNumber == createPlayerDto.JerseyNumber);
                    if (jerseyExists)
                        return BadRequest($"Jersey number {createPlayerDto.JerseyNumber} is already taken in this team");
                }

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
                    JerseyNumber = createPlayerDto.JerseyNumber
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
                    Height = player.Height,
                    Weight = player.Weight,
                    PreferredFoot = player.PreferredFoot,
                    JerseyNumber = player.JerseyNumber,
                    CreatedAt = player.CreatedAt,
                    UpdatedAt = player.UpdatedAt
                };

                return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, playerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating player");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(string id, UpdatePlayerDto updatePlayerDto)
        {
            try
            {
                var player = await _context.Players.FindAsync(id);
                if (player == null)
                    return NotFound($"Player with ID {id} not found");

                if (updatePlayerDto.Name != null) player.Name = updatePlayerDto.Name;
                if (updatePlayerDto.Position != null) player.Position = updatePlayerDto.Position;
                if (updatePlayerDto.Age.HasValue) player.Age = updatePlayerDto.Age.Value;
                if (updatePlayerDto.Nationality != null) player.Nationality = updatePlayerDto.Nationality;
                if (updatePlayerDto.TeamId != null) player.TeamId = updatePlayerDto.TeamId;
                if (updatePlayerDto.Height.HasValue) player.Height = updatePlayerDto.Height.Value;
                if (updatePlayerDto.Weight.HasValue) player.Weight = updatePlayerDto.Weight.Value;
                if (updatePlayerDto.PreferredFoot != null) player.PreferredFoot = updatePlayerDto.PreferredFoot;
                if (updatePlayerDto.JerseyNumber.HasValue) player.JerseyNumber = updatePlayerDto.JerseyNumber.Value;

                player.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating player {PlayerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(string id)
        {
            try
            {
                var player = await _context.Players.FindAsync(id);
                if (player == null)
                    return NotFound($"Player with ID {id} not found");

                _context.Players.Remove(player);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting player {PlayerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/statistics")]
        public async Task<ActionResult<IEnumerable<PlayerStatisticsDto>>> GetPlayerStatistics(string id)
        {
            try
            {
                var playerExists = await _context.Players.AnyAsync(p => p.Id == id);
                if (!playerExists)
                    return NotFound($"Player with ID {id} not found");

                var statistics = await _context.PlayerStatistics
                    .Where(ps => ps.PlayerId == id)
                    .Select(ps => new PlayerStatisticsDto
                    {
                        Id = ps.Id,
                        PlayerId = ps.PlayerId,
                        Season = ps.Season,
                        GamesPlayed = ps.GamesPlayed,
                        GamesStarted = ps.GamesStarted,
                        MinutesPlayed = ps.MinutesPlayed,
                        Goals = ps.Goals,
                        Assists = ps.Assists,
                        YellowCards = ps.YellowCards,
                        RedCards = ps.RedCards,
                        Tackles = ps.Tackles,
                        Interceptions = ps.Interceptions,
                        Clearances = ps.Clearances,
                        BlockedShots = ps.BlockedShots,
                        Fouls = ps.Fouls,
                        FoulsDrawn = ps.FoulsDrawn,
                        ShotsTotal = ps.ShotsTotal,
                        ShotsOnTarget = ps.ShotsOnTarget,
                        ShotAccuracy = ps.ShotAccuracy,
                        BigChancesCreated = ps.BigChancesCreated,
                        BigChancesMissed = ps.BigChancesMissed,
                        PassesAttempted = ps.PassesAttempted,
                        PassesCompleted = ps.PassesCompleted,
                        PassAccuracy = ps.PassAccuracy,
                        KeyPasses = ps.KeyPasses,
                        CrossesAttempted = ps.CrossesAttempted,
                        CrossesCompleted = ps.CrossesCompleted,
                        DistanceCovered = ps.DistanceCovered,
                        SprintsCompleted = ps.SprintsCompleted,
                        AverageRating = ps.AverageRating,
                        Wins = ps.Wins,
                        Draws = ps.Draws,
                        Losses = ps.Losses,
                        WinPercentage = ps.WinPercentage,
                        CreatedAt = ps.CreatedAt,
                        UpdatedAt = ps.UpdatedAt
                    })
                    .OrderByDescending(ps => ps.Season)
                    .ToListAsync();

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving player statistics for {PlayerId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}