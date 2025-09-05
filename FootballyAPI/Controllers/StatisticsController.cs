using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballyAPI.Data;
using FootballyAPI.Models;
using FootballyAPI.DTOs;

namespace FootballyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly FootballyDbContext _context;

        public StatisticsController(FootballyDbContext context)
        {
            _context = context;
        }

        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<PlayerStatisticsDto>>> GetPlayerStatistics(string playerId, [FromQuery] string? season = null)
        {
            var query = _context.PlayerStatistics
                .Where(ps => ps.PlayerId == playerId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(season))
                query = query.Where(ps => ps.Season == season);

            var statistics = await query
                .Select(ps => new PlayerStatisticsDto
                {
                    Season = ps.Season,
                    GamesPlayed = ps.GamesPlayed,
                    Goals = ps.Goals,
                    Assists = ps.Assists,
                    AverageRating = ps.AverageRating
                })
                .ToListAsync();

            return Ok(statistics);
        }

        [HttpGet("top-scorers")]
        public async Task<ActionResult<IEnumerable<TopScorerDto>>> GetTopScorers([FromQuery] string? season = null, [FromQuery] int limit = 10)
        {
            var query = _context.PlayerStatistics
                .Include(ps => ps.Player)
                .AsQueryable();

            if (!string.IsNullOrEmpty(season))
                query = query.Where(ps => ps.Season == season);

            var topScorers = await query
                .OrderByDescending(ps => ps.Goals)
                .Take(limit)
                .Select(ps => new TopScorerDto
                {
                    PlayerId = ps.PlayerId,
                    PlayerName = ps.Player.Name,
                    Position = ps.Player.Position,
                    Goals = ps.Goals,
                    Assists = ps.Assists,
                    GamesPlayed = ps.GamesPlayed,
                    Season = ps.Season
                })
                .ToListAsync();

            return Ok(topScorers);
        }

        [HttpGet("team/{teamId}/summary")]
        public async Task<ActionResult<TeamStatsSummaryDto>> GetTeamStatsSummary(string teamId, [FromQuery] string? season = null)
        {
            var players = await _context.Players
                .Include(p => p.Statistics)
                .Where(p => p.TeamId == teamId)
                .ToListAsync();

            var relevantStats = players
                .SelectMany(p => p.Statistics)
                .Where(s => string.IsNullOrEmpty(season) || s.Season == season)
                .ToList();

            if (!relevantStats.Any())
                return NotFound("No statistics found for the specified criteria");

            var summary = new TeamStatsSummaryDto
            {
                TeamId = teamId,
                Season = season ?? "All Seasons",
                TotalPlayers = players.Count,
                TotalGoals = relevantStats.Sum(s => s.Goals),
                TotalAssists = relevantStats.Sum(s => s.Assists),
                TotalGamesPlayed = relevantStats.Sum(s => s.GamesPlayed),
                AverageRating = relevantStats.Any() ? relevantStats.Average(s => s.AverageRating) : 0,
                TotalWins = relevantStats.Sum(s => s.Wins),
                TotalDraws = relevantStats.Sum(s => s.Draws),
                TotalLosses = relevantStats.Sum(s => s.Losses)
            };

            return Ok(summary);
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var totalTeams = await _context.Teams.CountAsync();
            var totalPlayers = await _context.Players.CountAsync();
            var totalMatches = await _context.Matches.CountAsync();
            var totalTournaments = await _context.Tournaments.CountAsync();
            
            var upcomingMatches = await _context.Matches
                .Where(m => m.Status == "Scheduled" && m.MatchDate > DateTime.UtcNow)
                .CountAsync();
            
            var recentMatches = await _context.Matches
                .Where(m => m.Status == "Finished" && m.MatchDate > DateTime.UtcNow.AddDays(-7))
                .CountAsync();

            var activeTournaments = await _context.Tournaments
                .Where(t => t.Status == "Active")
                .CountAsync();

            var pendingAnalyses = await _context.Matches
                .Where(m => m.Status == "Finished")
                .Where(m => !_context.MatchAnalyses.Any(ma => ma.MatchId == m.Id))
                .CountAsync();

            var dashboardStats = new DashboardStatsDto
            {
                TotalTeams = totalTeams,
                TotalPlayers = totalPlayers,
                TotalMatches = totalMatches,
                TotalTournaments = totalTournaments,
                UpcomingMatches = upcomingMatches,
                RecentMatches = recentMatches,
                ActiveTournaments = activeTournaments,
                PendingAnalyses = pendingAnalyses
            };

            return Ok(dashboardStats);
        }
    }

    // Additional DTOs for Statistics Controller
    public class TopScorerDto
    {
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int GamesPlayed { get; set; }
        public string Season { get; set; } = string.Empty;
    }

    public class TeamStatsSummaryDto
    {
        public string TeamId { get; set; } = string.Empty;
        public string Season { get; set; } = string.Empty;
        public int TotalPlayers { get; set; }
        public int TotalGoals { get; set; }
        public int TotalAssists { get; set; }
        public int TotalGamesPlayed { get; set; }
        public double AverageRating { get; set; }
        public int TotalWins { get; set; }
        public int TotalDraws { get; set; }
        public int TotalLosses { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalTeams { get; set; }
        public int TotalPlayers { get; set; }
        public int TotalMatches { get; set; }
        public int TotalTournaments { get; set; }
        public int UpcomingMatches { get; set; }
        public int RecentMatches { get; set; }
        public int ActiveTournaments { get; set; }
        public int PendingAnalyses { get; set; }
    }
}