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
    public class MatchAnalysisController : ControllerBase
    {
        private readonly FootballyDbContext _context;

        public MatchAnalysisController(FootballyDbContext context)
        {
            _context = context;
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<MatchAnalysisDto>> GetMatchAnalysis(string matchId)
        {
            var analysis = await _context.MatchAnalyses
                .Include(ma => ma.Match)
                .FirstOrDefaultAsync(ma => ma.MatchId == matchId);

            if (analysis == null)
                return NotFound("Match analysis not found");

            var manOfTheMatch = analysis.ManOfTheMatchId != null 
                ? await _context.Players.FindAsync(analysis.ManOfTheMatchId)
                : null;

            var analysisDto = new MatchAnalysisDto
            {
                Id = analysis.Id,
                MatchId = analysis.MatchId,
                AnalyzedBy = analysis.AnalyzedBy,
                ManOfTheMatchId = analysis.ManOfTheMatchId,
                ManOfTheMatchName = manOfTheMatch?.Name,
                ManOfTheMatchReason = analysis.ManOfTheMatchReason,
                MatchSummary = analysis.MatchSummary,
                KeyMoments = analysis.KeyMoments,
                TacticalAnalysis = analysis.TacticalAnalysis,
                HomeTeamAnalysis = analysis.HomeTeamAnalysis,
                AwayTeamAnalysis = analysis.AwayTeamAnalysis,
                HomeTeamPossession = analysis.HomeTeamPossession,
                AwayTeamPossession = analysis.AwayTeamPossession,
                HomeTeamShots = analysis.HomeTeamShots,
                AwayTeamShots = analysis.AwayTeamShots,
                HomeTeamShotsOnTarget = analysis.HomeTeamShotsOnTarget,
                AwayTeamShotsOnTarget = analysis.AwayTeamShotsOnTarget,
                HomeTeamCorners = analysis.HomeTeamCorners,
                AwayTeamCorners = analysis.AwayTeamCorners,
                HomeTeamFouls = analysis.HomeTeamFouls,
                AwayTeamFouls = analysis.AwayTeamFouls,
                HomeTeamYellowCards = analysis.HomeTeamYellowCards,
                AwayTeamYellowCards = analysis.AwayTeamYellowCards,
                HomeTeamRedCards = analysis.HomeTeamRedCards,
                AwayTeamRedCards = analysis.AwayTeamRedCards,
                HomeTeamOffsides = analysis.HomeTeamOffsides,
                AwayTeamOffsides = analysis.AwayTeamOffsides,
                MatchQualityRating = analysis.MatchQualityRating,
                MatchQualityComments = analysis.MatchQualityComments,
                IsCompleted = analysis.IsCompleted,
                CreatedAt = analysis.CreatedAt
            };

            return Ok(analysisDto);
        }

        [HttpPost("match/{matchId}")]
        public async Task<ActionResult<MatchAnalysisDto>> CreateMatchAnalysis(string matchId, [FromBody] CreateMatchAnalysisDto createAnalysisDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if match exists and is finished
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                return NotFound("Match not found");

            if (match.Status != "Finished")
                return BadRequest("Match must be finished to create analysis");

            // Check if analysis already exists
            var existingAnalysis = await _context.MatchAnalyses.FirstOrDefaultAsync(ma => ma.MatchId == matchId);
            if (existingAnalysis != null)
                return BadRequest("Match analysis already exists");

            // Validate Man of the Match if specified
            if (!string.IsNullOrEmpty(createAnalysisDto.ManOfTheMatchId))
            {
                var player = await _context.Players.FindAsync(createAnalysisDto.ManOfTheMatchId);
                if (player == null)
                    return BadRequest("Man of the Match player not found");

                // Verify player participated in the match
                var participated = await _context.MatchPerformances
                    .AnyAsync(mp => mp.MatchId == matchId && mp.PlayerId == createAnalysisDto.ManOfTheMatchId);
                
                if (!participated)
                    return BadRequest("Selected player did not participate in this match");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var analysis = new MatchAnalysis
            {
                MatchId = matchId,
                AnalyzedBy = userId,
                ManOfTheMatchId = createAnalysisDto.ManOfTheMatchId,
                ManOfTheMatchReason = createAnalysisDto.ManOfTheMatchReason,
                MatchSummary = createAnalysisDto.MatchSummary,
                KeyMoments = createAnalysisDto.KeyMoments,
                TacticalAnalysis = createAnalysisDto.TacticalAnalysis,
                HomeTeamAnalysis = createAnalysisDto.HomeTeamAnalysis,
                AwayTeamAnalysis = createAnalysisDto.AwayTeamAnalysis,
                HomeTeamPossession = createAnalysisDto.HomeTeamPossession,
                AwayTeamPossession = createAnalysisDto.AwayTeamPossession,
                HomeTeamShots = createAnalysisDto.HomeTeamShots,
                AwayTeamShots = createAnalysisDto.AwayTeamShots,
                HomeTeamShotsOnTarget = createAnalysisDto.HomeTeamShotsOnTarget,
                AwayTeamShotsOnTarget = createAnalysisDto.AwayTeamShotsOnTarget,
                HomeTeamCorners = createAnalysisDto.HomeTeamCorners,
                AwayTeamCorners = createAnalysisDto.AwayTeamCorners,
                HomeTeamFouls = createAnalysisDto.HomeTeamFouls,
                AwayTeamFouls = createAnalysisDto.AwayTeamFouls,
                HomeTeamYellowCards = createAnalysisDto.HomeTeamYellowCards,
                AwayTeamYellowCards = createAnalysisDto.AwayTeamYellowCards,
                HomeTeamRedCards = createAnalysisDto.HomeTeamRedCards,
                AwayTeamRedCards = createAnalysisDto.AwayTeamRedCards,
                HomeTeamOffsides = createAnalysisDto.HomeTeamOffsides,
                AwayTeamOffsides = createAnalysisDto.AwayTeamOffsides,
                MatchQualityRating = createAnalysisDto.MatchQualityRating,
                MatchQualityComments = createAnalysisDto.MatchQualityComments,
                IsCompleted = createAnalysisDto.IsCompleted
            };

            _context.MatchAnalyses.Add(analysis);
            await _context.SaveChangesAsync();

            // Update Man of the Match in player ratings if specified
            if (!string.IsNullOrEmpty(createAnalysisDto.ManOfTheMatchId))
            {
                var playerRating = await _context.PlayerRatings
                    .FirstOrDefaultAsync(pr => pr.MatchId == matchId && pr.PlayerId == createAnalysisDto.ManOfTheMatchId);
                
                if (playerRating != null)
                {
                    playerRating.IsManOfTheMatch = true;
                    await _context.SaveChangesAsync();
                }
            }

            return await GetMatchAnalysis(matchId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatchAnalysis(string id, [FromBody] UpdateMatchAnalysisDto updateAnalysisDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var analysis = await _context.MatchAnalyses.FindAsync(id);
            if (analysis == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only the analyzer or admin can update
            if (analysis.AnalyzedBy != userId && userRole != "Admin")
                return Forbid();

            // Validate Man of the Match if specified
            if (!string.IsNullOrEmpty(updateAnalysisDto.ManOfTheMatchId))
            {
                var player = await _context.Players.FindAsync(updateAnalysisDto.ManOfTheMatchId);
                if (player == null)
                    return BadRequest("Man of the Match player not found");

                var participated = await _context.MatchPerformances
                    .AnyAsync(mp => mp.MatchId == analysis.MatchId && mp.PlayerId == updateAnalysisDto.ManOfTheMatchId);
                
                if (!participated)
                    return BadRequest("Selected player did not participate in this match");
            }

            // Clear previous Man of the Match
            if (analysis.ManOfTheMatchId != updateAnalysisDto.ManOfTheMatchId)
            {
                if (!string.IsNullOrEmpty(analysis.ManOfTheMatchId))
                {
                    var oldRating = await _context.PlayerRatings
                        .FirstOrDefaultAsync(pr => pr.MatchId == analysis.MatchId && pr.PlayerId == analysis.ManOfTheMatchId);
                    
                    if (oldRating != null)
                    {
                        oldRating.IsManOfTheMatch = false;
                    }
                }
            }

            analysis.ManOfTheMatchId = updateAnalysisDto.ManOfTheMatchId;
            analysis.ManOfTheMatchReason = updateAnalysisDto.ManOfTheMatchReason;
            analysis.MatchSummary = updateAnalysisDto.MatchSummary;
            analysis.KeyMoments = updateAnalysisDto.KeyMoments;
            analysis.TacticalAnalysis = updateAnalysisDto.TacticalAnalysis;
            analysis.HomeTeamAnalysis = updateAnalysisDto.HomeTeamAnalysis;
            analysis.AwayTeamAnalysis = updateAnalysisDto.AwayTeamAnalysis;
            analysis.HomeTeamPossession = updateAnalysisDto.HomeTeamPossession;
            analysis.AwayTeamPossession = updateAnalysisDto.AwayTeamPossession;
            analysis.HomeTeamShots = updateAnalysisDto.HomeTeamShots;
            analysis.AwayTeamShots = updateAnalysisDto.AwayTeamShots;
            analysis.HomeTeamShotsOnTarget = updateAnalysisDto.HomeTeamShotsOnTarget;
            analysis.AwayTeamShotsOnTarget = updateAnalysisDto.AwayTeamShotsOnTarget;
            analysis.HomeTeamCorners = updateAnalysisDto.HomeTeamCorners;
            analysis.AwayTeamCorners = updateAnalysisDto.AwayTeamCorners;
            analysis.HomeTeamFouls = updateAnalysisDto.HomeTeamFouls;
            analysis.AwayTeamFouls = updateAnalysisDto.AwayTeamFouls;
            analysis.HomeTeamYellowCards = updateAnalysisDto.HomeTeamYellowCards;
            analysis.AwayTeamYellowCards = updateAnalysisDto.AwayTeamYellowCards;
            analysis.HomeTeamRedCards = updateAnalysisDto.HomeTeamRedCards;
            analysis.AwayTeamRedCards = updateAnalysisDto.AwayTeamRedCards;
            analysis.HomeTeamOffsides = updateAnalysisDto.HomeTeamOffsides;
            analysis.AwayTeamOffsides = updateAnalysisDto.AwayTeamOffsides;
            analysis.MatchQualityRating = updateAnalysisDto.MatchQualityRating;
            analysis.MatchQualityComments = updateAnalysisDto.MatchQualityComments;
            analysis.IsCompleted = updateAnalysisDto.IsCompleted;
            analysis.UpdatedAt = DateTime.UtcNow;

            // Update new Man of the Match
            if (!string.IsNullOrEmpty(updateAnalysisDto.ManOfTheMatchId))
            {
                var newRating = await _context.PlayerRatings
                    .FirstOrDefaultAsync(pr => pr.MatchId == analysis.MatchId && pr.PlayerId == updateAnalysisDto.ManOfTheMatchId);
                
                if (newRating != null)
                {
                    newRating.IsManOfTheMatch = true;
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<PendingMatchAnalysisDto>>> GetPendingMatchAnalyses()
        {
            var finishedMatches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.Status == "Finished")
                .Where(m => !_context.MatchAnalyses.Any(ma => ma.MatchId == m.Id))
                .OrderByDescending(m => m.MatchDate)
                .Select(m => new PendingMatchAnalysisDto
                {
                    MatchId = m.Id,
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamName = m.AwayTeam.Name,
                    HomeTeamScore = m.HomeTeamScore,
                    AwayTeamScore = m.AwayTeamScore,
                    MatchDate = m.MatchDate,
                    Competition = m.Competition,
                    Venue = m.Venue
                })
                .ToListAsync();

            return Ok(finishedMatches);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMatchAnalysis(string id)
        {
            var analysis = await _context.MatchAnalyses.FindAsync(id);
            if (analysis == null)
                return NotFound();

            _context.MatchAnalyses.Remove(analysis);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}