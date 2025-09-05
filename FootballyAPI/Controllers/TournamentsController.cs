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
    public class TournamentsController : ControllerBase
    {
        private readonly FootballyDbContext _context;

        public TournamentsController(FootballyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournaments()
        {
            var tournaments = await _context.Tournaments
                .Include(t => t.TournamentTeams)
                .Include(t => t.Matches)
                .Select(t => new TournamentDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Format = t.Format,
                    Status = t.Status,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    Season = t.Season,
                    MaxTeams = t.MaxTeams,
                    Organizer = t.Organizer,
                    LogoUrl = t.LogoUrl,
                    PrizePool = t.PrizePool,
                    Currency = t.Currency,
                    TeamsCount = t.TournamentTeams.Count,
                    MatchesCount = t.Matches.Count
                })
                .ToListAsync();

            return Ok(tournaments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDetailDto>> GetTournament(string id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.TournamentTeams)
                    .ThenInclude(tt => tt.Team)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.HomeTeam)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.AwayTeam)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
                return NotFound();

            var tournamentDetail = new TournamentDetailDto
            {
                Id = tournament.Id,
                Name = tournament.Name,
                Description = tournament.Description,
                Format = tournament.Format,
                Status = tournament.Status,
                StartDate = tournament.StartDate,
                EndDate = tournament.EndDate,
                Season = tournament.Season,
                MaxTeams = tournament.MaxTeams,
                Organizer = tournament.Organizer,
                LogoUrl = tournament.LogoUrl,
                Rules = tournament.Rules,
                PrizePool = tournament.PrizePool,
                Currency = tournament.Currency,
                Teams = tournament.TournamentTeams.Select(tt => new TournamentTeamDto
                {
                    TeamId = tt.TeamId,
                    TeamName = tt.Team.Name,
                    Group = tt.Group,
                    Points = tt.Points,
                    Wins = tt.Wins,
                    Draws = tt.Draws,
                    Losses = tt.Losses,
                    GoalsFor = tt.GoalsFor,
                    GoalsAgainst = tt.GoalsAgainst,
                    GoalDifference = tt.GoalDifference
                }).ToList(),
                Matches = tournament.Matches.Select(m => new MatchDto
                {
                    Id = m.Id,
                    HomeTeamId = m.HomeTeamId,
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamId = m.AwayTeamId,
                    AwayTeamName = m.AwayTeam.Name,
                    MatchDate = m.MatchDate,
                    HomeTeamScore = m.HomeTeamScore,
                    AwayTeamScore = m.AwayTeamScore,
                    Status = m.Status,
                    Round = m.Round,
                    Venue = m.Venue
                }).ToList()
            };

            return Ok(tournamentDetail);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TournamentDto>> CreateTournament([FromBody] CreateTournamentDto createTournamentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tournament = new Tournament
            {
                Name = createTournamentDto.Name,
                Description = createTournamentDto.Description,
                Format = createTournamentDto.Format,
                StartDate = createTournamentDto.StartDate,
                EndDate = createTournamentDto.EndDate,
                Season = createTournamentDto.Season,
                MaxTeams = createTournamentDto.MaxTeams,
                Organizer = createTournamentDto.Organizer,
                LogoUrl = createTournamentDto.LogoUrl,
                Rules = createTournamentDto.Rules,
                PrizePool = createTournamentDto.PrizePool,
                Currency = createTournamentDto.Currency
            };

            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync();

            var tournamentDto = new TournamentDto
            {
                Id = tournament.Id,
                Name = tournament.Name,
                Description = tournament.Description,
                Format = tournament.Format,
                Status = tournament.Status,
                StartDate = tournament.StartDate,
                EndDate = tournament.EndDate,
                Season = tournament.Season,
                MaxTeams = tournament.MaxTeams,
                Organizer = tournament.Organizer,
                LogoUrl = tournament.LogoUrl,
                PrizePool = tournament.PrizePool,
                Currency = tournament.Currency,
                TeamsCount = 0,
                MatchesCount = 0
            };

            return CreatedAtAction(nameof(GetTournament), new { id = tournament.Id }, tournamentDto);
        }

        [HttpPost("{id}/teams/{teamId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTeamToTournament(string id, string teamId, [FromBody] AddTeamToTournamentDto addTeamDto)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.TournamentTeams)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
                return NotFound("Tournament not found");

            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
                return NotFound("Team not found");

            if (tournament.TournamentTeams.Count >= tournament.MaxTeams)
                return BadRequest("Tournament is full");

            if (tournament.TournamentTeams.Any(tt => tt.TeamId == teamId))
                return BadRequest("Team is already in tournament");

            var tournamentTeam = new TournamentTeam
            {
                TournamentId = id,
                TeamId = teamId,
                Group = addTeamDto.Group ?? ""
            };

            _context.TournamentTeams.Add(tournamentTeam);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}/teams/{teamId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveTeamFromTournament(string id, string teamId)
        {
            var tournamentTeam = await _context.TournamentTeams
                .FirstOrDefaultAsync(tt => tt.TournamentId == id && tt.TeamId == teamId);

            if (tournamentTeam == null)
                return NotFound();

            _context.TournamentTeams.Remove(tournamentTeam);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTournamentStatus(string id, [FromBody] UpdateTournamentStatusDto statusDto)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null)
                return NotFound();

            tournament.Status = statusDto.Status;
            tournament.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}/standings")]
        public async Task<ActionResult<IEnumerable<TournamentTeamDto>>> GetTournamentStandings(string id)
        {
            var standings = await _context.TournamentTeams
                .Include(tt => tt.Team)
                .Where(tt => tt.TournamentId == id)
                .OrderByDescending(tt => tt.Points)
                .ThenByDescending(tt => tt.GoalDifference)
                .ThenByDescending(tt => tt.GoalsFor)
                .Select(tt => new TournamentTeamDto
                {
                    TeamId = tt.TeamId,
                    TeamName = tt.Team.Name,
                    Group = tt.Group,
                    Points = tt.Points,
                    Wins = tt.Wins,
                    Draws = tt.Draws,
                    Losses = tt.Losses,
                    GoalsFor = tt.GoalsFor,
                    GoalsAgainst = tt.GoalsAgainst,
                    GoalDifference = tt.GoalDifference
                })
                .ToListAsync();

            return Ok(standings);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTournament(string id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null)
                return NotFound();

            _context.Tournaments.Remove(tournament);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}