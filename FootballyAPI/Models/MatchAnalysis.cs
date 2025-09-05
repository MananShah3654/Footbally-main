using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.Models
{
    public class MatchAnalysis
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string MatchId { get; set; } = string.Empty;

        [Required]
        public string AnalyzedBy { get; set; } = string.Empty; // User ID of the analyst/referee

        // Man of the Match
        public string? ManOfTheMatchId { get; set; }

        [StringLength(1000)]
        public string ManOfTheMatchReason { get; set; } = string.Empty;

        // Match summary
        [StringLength(2000)]
        public string MatchSummary { get; set; } = string.Empty;

        [StringLength(1000)]
        public string KeyMoments { get; set; } = string.Empty;

        [StringLength(1000)]
        public string TacticalAnalysis { get; set; } = string.Empty;

        // Team performance analysis
        [StringLength(1000)]
        public string HomeTeamAnalysis { get; set; } = string.Empty;

        [StringLength(1000)]
        public string AwayTeamAnalysis { get; set; } = string.Empty;

        // Match statistics
        public int HomeTeamPossession { get; set; } = 50; // Percentage
        public int AwayTeamPossession { get; set; } = 50;

        public int HomeTeamShots { get; set; } = 0;
        public int AwayTeamShots { get; set; } = 0;

        public int HomeTeamShotsOnTarget { get; set; } = 0;
        public int AwayTeamShotsOnTarget { get; set; } = 0;

        public int HomeTeamCorners { get; set; } = 0;
        public int AwayTeamCorners { get; set; } = 0;

        public int HomeTeamFouls { get; set; } = 0;
        public int AwayTeamFouls { get; set; } = 0;

        public int HomeTeamYellowCards { get; set; } = 0;
        public int AwayTeamYellowCards { get; set; } = 0;

        public int HomeTeamRedCards { get; set; } = 0;
        public int AwayTeamRedCards { get; set; } = 0;

        public int HomeTeamOffsides { get; set; } = 0;
        public int AwayTeamOffsides { get; set; } = 0;

        // Overall match quality rating
        [Range(1.0, 10.0)]
        public double MatchQualityRating { get; set; } = 6.0;

        [StringLength(500)]
        public string MatchQualityComments { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}