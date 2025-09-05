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
    public class MatchesController : ControllerBase
    {
        private readonly FootballyDbContext _context;

        public MatchesController(FootballyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatches([FromQuery] string? status = null, [FromQuery] string? tournamentId = null)
        {
            var query = _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(m => m.Status == status);

            if (!string.IsNullOrEmpty(tournamentId))
                query = query.Where(m => m.TournamentId == tournamentId);

            var matches = await query
                .OrderByDescending(m => m.MatchDate)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    HomeTeamId = m.HomeTeamId,
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamId = m.AwayTeamId,
                    AwayTeamName = m.AwayTeam.Name,
                    MatchDate = m.MatchDate,
                    Competition = m.Competition,
                    Season = m.Season,
                    HomeTeamScore = m.HomeTeamScore,
                    AwayTeamScore = m.AwayTeamScore,
                    Status = m.Status,
                    Venue = m.Venue,
                    Referee = m.Referee,
                    Round = m.Round,
                    TournamentId = m.TournamentId
                })
                .ToListAsync();

            return Ok(matches);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDetailDto>> GetMatch(string id)
        {
            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.PlayerPerformances)
                .Include(m => m.PlayerRatings)
                .Include(m => m.MatchAnalysis)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound();

            var matchDetail = new MatchDetailDto
            {
                Id = match.Id,
                HomeTeamId = match.HomeTeamId,
                HomeTeamName = match.HomeTeam.Name,
                AwayTeamId = match.AwayTeamId,
                AwayTeamName = match.AwayTeam.Name,
                MatchDate = match.MatchDate,
                Competition = match.Competition,
                Season = match.Season,
                HomeTeamScore = match.HomeTeamScore,
                AwayTeamScore = match.AwayTeamScore,
                Status = match.Status,
                Venue = match.Venue,
                Referee = match.Referee,
                Attendance = match.Attendance,
                Weather = match.Weather,
                MinutesPlayed = match.MinutesPlayed,
                Round = match.Round,
                TournamentId = match.TournamentId,
                PlayerPerformances = match.PlayerPerformances.Select(pp => new MatchPerformanceDto
                {
                    PlayerId = pp.PlayerId,
                    MatchId = pp.MatchId,
                    MinutesPlayed = pp.MinutesPlayed,
                    Goals = pp.Goals,
                    Assists = pp.Assists,
                    YellowCards = pp.YellowCards,
                    RedCards = pp.RedCards
                }).ToList(),
                HasAnalysis = match.MatchAnalysis != null,
                AnalysisCompleted = match.MatchAnalysis?.IsCompleted ?? false
            };

            return Ok(matchDetail);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MatchDto>> CreateMatch([FromBody] CreateMatchDto createMatchDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate teams exist
            var homeTeam = await _context.Teams.FindAsync(createMatchDto.HomeTeamId);
            var awayTeam = await _context.Teams.FindAsync(createMatchDto.AwayTeamId);

            if (homeTeam == null || awayTeam == null)
                return BadRequest("Invalid team(s)");

            if (createMatchDto.HomeTeamId == createMatchDto.AwayTeamId)
                return BadRequest("A team cannot play against itself");

            // Validate tournament if specified
            if (!string.IsNullOrEmpty(createMatchDto.TournamentId))
            {
                var tournament = await _context.Tournaments.FindAsync(createMatchDto.TournamentId);
                if (tournament == null)
                    return BadRequest("Tournament not found");
            }

            var match = new Match
            {
                HomeTeamId = createMatchDto.HomeTeamId,
                AwayTeamId = createMatchDto.AwayTeamId,
                MatchDate = createMatchDto.MatchDate,
                Competition = createMatchDto.Competition,
                Season = createMatchDto.Season,
                Venue = createMatchDto.Venue,
                Referee = createMatchDto.Referee,
                TournamentId = createMatchDto.TournamentId,
                Round = createMatchDto.Round
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            var matchDto = new MatchDto
            {
                Id = match.Id,
                HomeTeamId = match.HomeTeamId,
                HomeTeamName = homeTeam.Name,
                AwayTeamId = match.AwayTeamId,
                AwayTeamName = awayTeam.Name,
                MatchDate = match.MatchDate,
                Competition = match.Competition,
                Season = match.Season,
                HomeTeamScore = match.HomeTeamScore,
                AwayTeamScore = match.AwayTeamScore,
                Status = match.Status,
                Venue = match.Venue,
                Referee = match.Referee,
                Round = match.Round,
                TournamentId = match.TournamentId
            };

            return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, matchDto);
        }

        [HttpPut("{id}/score")]
        [Authorize(Roles = "Admin,Referee")]
        public async Task<IActionResult> UpdateMatchScore(string id, [FromBody] UpdateMatchScoreDto scoreDto)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
                return NotFound();

            match.HomeTeamScore = scoreDto.HomeTeamScore;
            match.AwayTeamScore = scoreDto.AwayTeamScore;
            match.Status = scoreDto.Status;
            match.MinutesPlayed = scoreDto.MinutesPlayed;
            match.UpdatedAt = DateTime.UtcNow;

            // Update tournament standings if this is a tournament match
            if (!string.IsNullOrEmpty(match.TournamentId) && match.Status == "Finished")
            {
                await UpdateTournamentStandings(match);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Referee")]
        public async Task<IActionResult> UpdateMatchStatus(string id, [FromBody] UpdateMatchStatusDto statusDto)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
                return NotFound();

            match.Status = statusDto.Status;
            match.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetUpcomingMatches([FromQuery] int limit = 10)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.Status == "Scheduled" && m.MatchDate > DateTime.UtcNow)
                .OrderBy(m => m.MatchDate)
                .Take(limit)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    HomeTeamId = m.HomeTeamId,
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamId = m.AwayTeamId,
                    AwayTeamName = m.AwayTeam.Name,
                    MatchDate = m.MatchDate,
                    Competition = m.Competition,
                    Status = m.Status,
                    Venue = m.Venue,
                    Round = m.Round
                })
                .ToListAsync();

            return Ok(matches);
        }

        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetRecentMatches([FromQuery] int limit = 10)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.Status == "Finished")
                .OrderByDescending(m => m.MatchDate)
                .Take(limit)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    HomeTeamId = m.HomeTeamId,
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamId = m.AwayTeamId,
                    AwayTeamName = m.AwayTeam.Name,
                    MatchDate = m.MatchDate,
                    Competition = m.Competition,
                    HomeTeamScore = m.HomeTeamScore,
                    AwayTeamScore = m.AwayTeamScore,
                    Status = m.Status,
                    Venue = m.Venue,
                    Round = m.Round
                })
                .ToListAsync();

            return Ok(matches);
        }

        private async Task UpdateTournamentStandings(Match match)
        {
            var homeTeamStats = await _context.TournamentTeams
                .FirstOrDefaultAsync(tt => tt.TournamentId == match.TournamentId && tt.TeamId == match.HomeTeamId);
            
            var awayTeamStats = await _context.TournamentTeams
                .FirstOrDefaultAsync(tt => tt.TournamentId == match.TournamentId && tt.TeamId == match.AwayTeamId);

            if (homeTeamStats != null && awayTeamStats != null)
            {
                // Update goals
                homeTeamStats.GoalsFor += match.HomeTeamScore;
                homeTeamStats.GoalsAgainst += match.AwayTeamScore;
                awayTeamStats.GoalsFor += match.AwayTeamScore;
                awayTeamStats.GoalsAgainst += match.HomeTeamScore;

                // Update goal difference
                homeTeamStats.GoalDifference = homeTeamStats.GoalsFor - homeTeamStats.GoalsAgainst;
                awayTeamStats.GoalDifference = awayTeamStats.GoalsFor - awayTeamStats.GoalsAgainst;

                // Update wins/draws/losses and points
                if (match.HomeTeamScore > match.AwayTeamScore)
                {
                    homeTeamStats.Wins++;
                    homeTeamStats.Points += 3;
                    awayTeamStats.Losses++;
                }
                else if (match.AwayTeamScore > match.HomeTeamScore)
                {
                    awayTeamStats.Wins++;
                    awayTeamStats.Points += 3;
                    homeTeamStats.Losses++;
                }
                else
                {
                    homeTeamStats.Draws++;
                    homeTeamStats.Points += 1;
                    awayTeamStats.Draws++;
                    awayTeamStats.Points += 1;
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMatch(string id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
                return NotFound();

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}