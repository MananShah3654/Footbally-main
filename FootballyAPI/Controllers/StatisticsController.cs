using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballyAPI.Data;
using FootballyAPI.Models;
using FootballyAPI.DTOs;

namespace FootballyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly FootballyDbContext _context;
        private readonly ILogger<StatisticsController> _logger;

        public StatisticsController(FootballyDbContext context, ILogger<StatisticsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerStatisticsDto>>> GetStatistics(
            [FromQuery] string? playerId = null,
            [FromQuery] string? season = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = _context.PlayerStatistics.AsQueryable();

                if (!string.IsNullOrEmpty(playerId))
                    query = query.Where(ps => ps.PlayerId == playerId);

                if (!string.IsNullOrEmpty(season))
                    query = query.Where(ps => ps.Season == season);

                var totalCount = await query.CountAsync();
                var statistics = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
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
                    .OrderBy(ps => ps.Season)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerStatisticsDto>> GetStatistics(string id)
        {
            try
            {
                var statistics = await _context.PlayerStatistics
                    .Where(ps => ps.Id == id)
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
                    .FirstOrDefaultAsync();

                if (statistics == null)
                    return NotFound($"Statistics with ID {id} not found");

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics {StatisticsId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PlayerStatisticsDto>> CreateStatistics(CreatePlayerStatisticsDto createStatisticsDto)
        {
            try
            {
                // Check if player exists
                var playerExists = await _context.Players.AnyAsync(p => p.Id == createStatisticsDto.PlayerId);
                if (!playerExists)
                    return BadRequest($"Player with ID {createStatisticsDto.PlayerId} not found");

                // Check if statistics for this player and season already exist
                var existingStats = await _context.PlayerStatistics
                    .AnyAsync(ps => ps.PlayerId == createStatisticsDto.PlayerId && ps.Season == createStatisticsDto.Season);
                if (existingStats)
                    return BadRequest($"Statistics for player {createStatisticsDto.PlayerId} in season {createStatisticsDto.Season} already exist");

                var statistics = new PlayerStatistics
                {
                    PlayerId = createStatisticsDto.PlayerId,
                    Season = createStatisticsDto.Season,
                    GamesPlayed = createStatisticsDto.GamesPlayed,
                    GamesStarted = createStatisticsDto.GamesStarted,
                    MinutesPlayed = createStatisticsDto.MinutesPlayed,
                    Goals = createStatisticsDto.Goals,
                    Assists = createStatisticsDto.Assists,
                    YellowCards = createStatisticsDto.YellowCards,
                    RedCards = createStatisticsDto.RedCards,
                    Tackles = createStatisticsDto.Tackles,
                    Interceptions = createStatisticsDto.Interceptions,
                    Clearances = createStatisticsDto.Clearances,
                    BlockedShots = createStatisticsDto.BlockedShots,
                    Fouls = createStatisticsDto.Fouls,
                    FoulsDrawn = createStatisticsDto.FoulsDrawn,
                    ShotsTotal = createStatisticsDto.ShotsTotal,
                    ShotsOnTarget = createStatisticsDto.ShotsOnTarget,
                    BigChancesCreated = createStatisticsDto.BigChancesCreated,
                    BigChancesMissed = createStatisticsDto.BigChancesMissed,
                    PassesAttempted = createStatisticsDto.PassesAttempted,
                    PassesCompleted = createStatisticsDto.PassesCompleted,
                    KeyPasses = createStatisticsDto.KeyPasses,
                    CrossesAttempted = createStatisticsDto.CrossesAttempted,
                    CrossesCompleted = createStatisticsDto.CrossesCompleted,
                    DistanceCovered = createStatisticsDto.DistanceCovered,
                    SprintsCompleted = createStatisticsDto.SprintsCompleted,
                    AverageRating = createStatisticsDto.AverageRating,
                    Wins = createStatisticsDto.Wins,
                    Draws = createStatisticsDto.Draws,
                    Losses = createStatisticsDto.Losses
                };

                _context.PlayerStatistics.Add(statistics);
                await _context.SaveChangesAsync();

                var statisticsDto = new PlayerStatisticsDto
                {
                    Id = statistics.Id,
                    PlayerId = statistics.PlayerId,
                    Season = statistics.Season,
                    GamesPlayed = statistics.GamesPlayed,
                    GamesStarted = statistics.GamesStarted,
                    MinutesPlayed = statistics.MinutesPlayed,
                    Goals = statistics.Goals,
                    Assists = statistics.Assists,
                    YellowCards = statistics.YellowCards,
                    RedCards = statistics.RedCards,
                    Tackles = statistics.Tackles,
                    Interceptions = statistics.Interceptions,
                    Clearances = statistics.Clearances,
                    BlockedShots = statistics.BlockedShots,
                    Fouls = statistics.Fouls,
                    FoulsDrawn = statistics.FoulsDrawn,
                    ShotsTotal = statistics.ShotsTotal,
                    ShotsOnTarget = statistics.ShotsOnTarget,
                    ShotAccuracy = statistics.ShotAccuracy,
                    BigChancesCreated = statistics.BigChancesCreated,
                    BigChancesMissed = statistics.BigChancesMissed,
                    PassesAttempted = statistics.PassesAttempted,
                    PassesCompleted = statistics.PassesCompleted,
                    PassAccuracy = statistics.PassAccuracy,
                    KeyPasses = statistics.KeyPasses,
                    CrossesAttempted = statistics.CrossesAttempted,
                    CrossesCompleted = statistics.CrossesCompleted,
                    DistanceCovered = statistics.DistanceCovered,
                    SprintsCompleted = statistics.SprintsCompleted,
                    AverageRating = statistics.AverageRating,
                    Wins = statistics.Wins,
                    Draws = statistics.Draws,
                    Losses = statistics.Losses,
                    WinPercentage = statistics.WinPercentage,
                    CreatedAt = statistics.CreatedAt,
                    UpdatedAt = statistics.UpdatedAt
                };

                return CreatedAtAction(nameof(GetStatistics), new { id = statistics.Id }, statisticsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatistics(string id)
        {
            try
            {
                var statistics = await _context.PlayerStatistics.FindAsync(id);
                if (statistics == null)
                    return NotFound($"Statistics with ID {id} not found");

                _context.PlayerStatistics.Remove(statistics);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting statistics {StatisticsId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}