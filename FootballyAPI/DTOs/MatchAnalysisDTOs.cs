using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.DTOs
{
    public class MatchAnalysisDto
    {
        public string Id { get; set; } = string.Empty;
        public string MatchId { get; set; } = string.Empty;
        public string AnalyzedBy { get; set; } = string.Empty;
        public string? ManOfTheMatchId { get; set; }
        public string? ManOfTheMatchName { get; set; }
        public string ManOfTheMatchReason { get; set; } = string.Empty;
        public string MatchSummary { get; set; } = string.Empty;
        public string KeyMoments { get; set; } = string.Empty;
        public string TacticalAnalysis { get; set; } = string.Empty;
        public string HomeTeamAnalysis { get; set; } = string.Empty;
        public string AwayTeamAnalysis { get; set; } = string.Empty;
        public int HomeTeamPossession { get; set; }
        public int AwayTeamPossession { get; set; }
        public int HomeTeamShots { get; set; }
        public int AwayTeamShots { get; set; }
        public int HomeTeamShotsOnTarget { get; set; }
        public int AwayTeamShotsOnTarget { get; set; }
        public int HomeTeamCorners { get; set; }
        public int AwayTeamCorners { get; set; }
        public int HomeTeamFouls { get; set; }
        public int AwayTeamFouls { get; set; }
        public int HomeTeamYellowCards { get; set; }
        public int AwayTeamYellowCards { get; set; }
        public int HomeTeamRedCards { get; set; }
        public int AwayTeamRedCards { get; set; }
        public int HomeTeamOffsides { get; set; }
        public int AwayTeamOffsides { get; set; }
        public double MatchQualityRating { get; set; }
        public string MatchQualityComments { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMatchAnalysisDto
    {
        public string? ManOfTheMatchId { get; set; }

        [StringLength(1000)]
        public string ManOfTheMatchReason { get; set; } = string.Empty;

        [StringLength(2000)]
        public string MatchSummary { get; set; } = string.Empty;

        [StringLength(1000)]
        public string KeyMoments { get; set; } = string.Empty;

        [StringLength(1000)]
        public string TacticalAnalysis { get; set; } = string.Empty;

        [StringLength(1000)]
        public string HomeTeamAnalysis { get; set; } = string.Empty;

        [StringLength(1000)]
        public string AwayTeamAnalysis { get; set; } = string.Empty;

        [Range(0, 100)]
        public int HomeTeamPossession { get; set; } = 50;

        [Range(0, 100)]
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

        [Range(1.0, 10.0)]
        public double MatchQualityRating { get; set; } = 6.0;

        [StringLength(500)]
        public string MatchQualityComments { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;
    }

    public class UpdateMatchAnalysisDto : CreateMatchAnalysisDto
    {
    }

    public class PendingMatchAnalysisDto
    {
        public string MatchId { get; set; } = string.Empty;
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public DateTime MatchDate { get; set; }
        public string Competition { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
    }
}