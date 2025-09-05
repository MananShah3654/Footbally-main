using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballyAPI.Data;
using FootballyAPI.Models;
using FootballyAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FootballyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Referee")]
    public class PlayerRatingsController : ControllerBase
    {
        private readonly FootballyDbContext _context;

        public PlayerRatingsController(FootballyDbContext context)
        {
            _context = context;
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetMatchPlayerRatings(string matchId)
        {
            var ratings = await _context.PlayerRatings
                .Include(pr => pr.Player)
                .Where(pr => pr.MatchId == matchId)
                .Select(pr => new PlayerRatingDto
                {
                    Id = pr.Id,
                    MatchId = pr.MatchId,
                    PlayerId = pr.PlayerId,
                    PlayerName = pr.Player.Name,
                    RatedBy = pr.RatedBy,
                    Rating = pr.Rating,
                    AttackingRating = pr.AttackingRating,
                    DefendingRating = pr.DefendingRating,
                    PassingRating = pr.PassingRating,
                    PhysicalRating = pr.PhysicalRating,
                    MentalRating = pr.MentalRating,
                    Comments = pr.Comments,
                    PositiveHighlights = pr.PositiveHighlights,
                    AreasForImprovement = pr.AreasForImprovement,
                    IsManOfTheMatch = pr.IsManOfTheMatch,
                    CreatedAt = pr.CreatedAt
                })
                .OrderByDescending(pr => pr.Rating)
                .ToListAsync();

            return Ok(ratings);
        }

        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetPlayerRatings(string playerId, [FromQuery] int limit = 10)
        {
            var ratings = await _context.PlayerRatings
                .Include(pr => pr.Match)
                    .ThenInclude(m => m.HomeTeam)
                .Include(pr => pr.Match)
                    .ThenInclude(m => m.AwayTeam)
                .Where(pr => pr.PlayerId == playerId)
                .OrderByDescending(pr => pr.CreatedAt)
                .Take(limit)
                .Select(pr => new PlayerRatingWithMatchDto
                {
                    Id = pr.Id,
                    MatchId = pr.MatchId,
                    PlayerId = pr.PlayerId,
                    RatedBy = pr.RatedBy,
                    Rating = pr.Rating,
                    AttackingRating = pr.AttackingRating,
                    DefendingRating = pr.DefendingRating,
                    PassingRating = pr.PassingRating,
                    PhysicalRating = pr.PhysicalRating,
                    MentalRating = pr.MentalRating,
                    Comments = pr.Comments,
                    PositiveHighlights = pr.PositiveHighlights,
                    AreasForImprovement = pr.AreasForImprovement,
                    IsManOfTheMatch = pr.IsManOfTheMatch,
                    MatchDate = pr.Match.MatchDate,
                    HomeTeamName = pr.Match.HomeTeam.Name,
                    AwayTeamName = pr.Match.AwayTeam.Name,
                    Competition = pr.Match.Competition,
                    CreatedAt = pr.CreatedAt
                })
                .ToListAsync();

            return Ok(ratings);
        }

        [HttpPost]
        public async Task<ActionResult<PlayerRatingDto>> CreatePlayerRating([FromBody] CreatePlayerRatingDto createRatingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate match exists and is finished
            var match = await _context.Matches.FindAsync(createRatingDto.MatchId);
            if (match == null)
                return NotFound("Match not found");

            if (match.Status != "Finished")
                return BadRequest("Match must be finished to rate players");

            // Validate player exists
            var player = await _context.Players.FindAsync(createRatingDto.PlayerId);
            if (player == null)
                return NotFound("Player not found");

            // Check if player participated in the match
            var participated = await _context.MatchPerformances
                .AnyAsync(mp => mp.MatchId == createRatingDto.MatchId && mp.PlayerId == createRatingDto.PlayerId);

            if (!participated)
                return BadRequest("Player did not participate in this match");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Check if rating already exists for this player in this match by this user
            var existingRating = await _context.PlayerRatings
                .FirstOrDefaultAsync(pr => pr.MatchId == createRatingDto.MatchId && 
                                          pr.PlayerId == createRatingDto.PlayerId && 
                                          pr.RatedBy == userId);

            if (existingRating != null)
                return BadRequest("You have already rated this player for this match");

            var rating = new PlayerRating
            {
                MatchId = createRatingDto.MatchId,
                PlayerId = createRatingDto.PlayerId,
                RatedBy = userId,
                Rating = createRatingDto.Rating,
                AttackingRating = createRatingDto.AttackingRating,
                DefendingRating = createRatingDto.DefendingRating,
                PassingRating = createRatingDto.PassingRating,
                PhysicalRating = createRatingDto.PhysicalRating,
                MentalRating = createRatingDto.MentalRating,
                Comments = createRatingDto.Comments,
                PositiveHighlights = createRatingDto.PositiveHighlights,
                AreasForImprovement = createRatingDto.AreasForImprovement
            };

            _context.PlayerRatings.Add(rating);
            await _context.SaveChangesAsync();

            var ratingDto = new PlayerRatingDto
            {
                Id = rating.Id,
                MatchId = rating.MatchId,
                PlayerId = rating.PlayerId,
                PlayerName = player.Name,
                RatedBy = rating.RatedBy,
                Rating = rating.Rating,
                AttackingRating = rating.AttackingRating,
                DefendingRating = rating.DefendingRating,
                PassingRating = rating.PassingRating,
                PhysicalRating = rating.PhysicalRating,
                MentalRating = rating.MentalRating,
                Comments = rating.Comments,
                PositiveHighlights = rating.PositiveHighlights,
                AreasForImprovement = rating.AreasForImprovement,
                IsManOfTheMatch = rating.IsManOfTheMatch,
                CreatedAt = rating.CreatedAt
            };

            return CreatedAtAction(nameof(GetPlayerRating), new { id = rating.Id }, ratingDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerRatingDto>> GetPlayerRating(string id)
        {
            var rating = await _context.PlayerRatings
                .Include(pr => pr.Player)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (rating == null)
                return NotFound();

            var ratingDto = new PlayerRatingDto
            {
                Id = rating.Id,
                MatchId = rating.MatchId,
                PlayerId = rating.PlayerId,
                PlayerName = rating.Player.Name,
                RatedBy = rating.RatedBy,
                Rating = rating.Rating,
                AttackingRating = rating.AttackingRating,
                DefendingRating = rating.DefendingRating,
                PassingRating = rating.PassingRating,
                PhysicalRating = rating.PhysicalRating,
                MentalRating = rating.MentalRating,
                Comments = rating.Comments,
                PositiveHighlights = rating.PositiveHighlights,
                AreasForImprovement = rating.AreasForImprovement,
                IsManOfTheMatch = rating.IsManOfTheMatch,
                CreatedAt = rating.CreatedAt
            };

            return Ok(ratingDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayerRating(string id, [FromBody] UpdatePlayerRatingDto updateRatingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rating = await _context.PlayerRatings.FindAsync(id);
            if (rating == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only the rater or admin can update
            if (rating.RatedBy != userId && userRole != "Admin")
                return Forbid();

            rating.Rating = updateRatingDto.Rating;
            rating.AttackingRating = updateRatingDto.AttackingRating;
            rating.DefendingRating = updateRatingDto.DefendingRating;
            rating.PassingRating = updateRatingDto.PassingRating;
            rating.PhysicalRating = updateRatingDto.PhysicalRating;
            rating.MentalRating = updateRatingDto.MentalRating;
            rating.Comments = updateRatingDto.Comments;
            rating.PositiveHighlights = updateRatingDto.PositiveHighlights;
            rating.AreasForImprovement = updateRatingDto.AreasForImprovement;
            rating.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("top-rated")]
        public async Task<ActionResult<IEnumerable<TopRatedPlayerDto>>> GetTopRatedPlayers([FromQuery] int limit = 10, [FromQuery] string? season = null)
        {
            var query = _context.PlayerRatings
                .Include(pr => pr.Player)
                .Include(pr => pr.Match)
                .AsQueryable();

            if (!string.IsNullOrEmpty(season))
                query = query.Where(pr => pr.Match.Season == season);

            var topRated = await query
                .GroupBy(pr => new { pr.PlayerId, pr.Player.Name, pr.Player.Position })
                .Select(g => new TopRatedPlayerDto
                {
                    PlayerId = g.Key.PlayerId,
                    PlayerName = g.Key.Name,
                    Position = g.Key.Position,
                    AverageRating = g.Average(pr => pr.Rating),
                    TotalRatings = g.Count(),
                    ManOfTheMatchCount = g.Count(pr => pr.IsManOfTheMatch),
                    HighestRating = g.Max(pr => pr.Rating),
                    LowestRating = g.Min(pr => pr.Rating)
                })
                .OrderByDescending(tr => tr.AverageRating)
                .Take(limit)
                .ToListAsync();

            return Ok(topRated);
        }

        [HttpGet("man-of-match")]
        public async Task<ActionResult<IEnumerable<ManOfTheMatchDto>>> GetManOfTheMatchWinners([FromQuery] int limit = 10, [FromQuery] string? season = null)
        {
            var query = _context.PlayerRatings
                .Include(pr => pr.Player)
                .Include(pr => pr.Match)
                    .ThenInclude(m => m.HomeTeam)
                .Include(pr => pr.Match)
                    .ThenInclude(m => m.AwayTeam)
                .Where(pr => pr.IsManOfTheMatch)
                .AsQueryable();

            if (!string.IsNullOrEmpty(season))
                query = query.Where(pr => pr.Match.Season == season);

            var manOfTheMatchWinners = await query
                .OrderByDescending(pr => pr.Match.MatchDate)
                .Take(limit)
                .Select(pr => new ManOfTheMatchDto
                {
                    PlayerId = pr.PlayerId,
                    PlayerName = pr.Player.Name,
                    Position = pr.Player.Position,
                    MatchId = pr.MatchId,
                    MatchDate = pr.Match.MatchDate,
                    HomeTeamName = pr.Match.HomeTeam.Name,
                    AwayTeamName = pr.Match.AwayTeam.Name,
                    Competition = pr.Match.Competition,
                    Rating = pr.Rating,
                    Reason = pr.Comments
                })
                .ToListAsync();

            return Ok(manOfTheMatchWinners);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlayerRating(string id)
        {
            var rating = await _context.PlayerRatings.FindAsync(id);
            if (rating == null)
                return NotFound();

            _context.PlayerRatings.Remove(rating);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}