using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.DTOs
{
    public class PlayerRatingDto
    {
        public string Id { get; set; } = string.Empty;
        public string MatchId { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string RatedBy { get; set; } = string.Empty;
        public double Rating { get; set; }
        public double AttackingRating { get; set; }
        public double DefendingRating { get; set; }
        public double PassingRating { get; set; }
        public double PhysicalRating { get; set; }
        public double MentalRating { get; set; }
        public string Comments { get; set; } = string.Empty;
        public string PositiveHighlights { get; set; } = string.Empty;
        public string AreasForImprovement { get; set; } = string.Empty;
        public bool IsManOfTheMatch { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PlayerRatingWithMatchDto : PlayerRatingDto
    {
        public DateTime MatchDate { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;
        public string Competition { get; set; } = string.Empty;
    }

    public class CreatePlayerRatingDto
    {
        [Required]
        public string MatchId { get; set; } = string.Empty;

        [Required]
        public string PlayerId { get; set; } = string.Empty;

        [Range(1.0, 10.0)]
        public double Rating { get; set; } = 6.0;

        [Range(1.0, 10.0)]
        public double AttackingRating { get; set; } = 6.0;

        [Range(1.0, 10.0)]
        public double DefendingRating { get; set; } = 6.0;

        [Range(1.0, 10.0)]
        public double PassingRating { get; set; } = 6.0;

        [Range(1.0, 10.0)]
        public double PhysicalRating { get; set; } = 6.0;

        [Range(1.0, 10.0)]
        public double MentalRating { get; set; } = 6.0;

        [StringLength(1000)]
        public string Comments { get; set; } = string.Empty;

        [StringLength(500)]
        public string PositiveHighlights { get; set; } = string.Empty;

        [StringLength(500)]
        public string AreasForImprovement { get; set; } = string.Empty;
    }

    public class UpdatePlayerRatingDto : CreatePlayerRatingDto
    {
    }

    public class TopRatedPlayerDto
    {
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int ManOfTheMatchCount { get; set; }
        public double HighestRating { get; set; }
        public double LowestRating { get; set; }
    }

    public class ManOfTheMatchDto
    {
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string MatchId { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;
        public string Competition { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}