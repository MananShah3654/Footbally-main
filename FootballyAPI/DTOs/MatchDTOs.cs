using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.DTOs
{
    public class MatchDto
    {
        public string Id { get; set; } = string.Empty;
        public string HomeTeamId { get; set; } = string.Empty;
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamId { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public string Competition { get; set; } = string.Empty;
        public string Season { get; set; } = string.Empty;
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public string Referee { get; set; } = string.Empty;
        public string Round { get; set; } = string.Empty;
        public string? TournamentId { get; set; }
    }

    public class MatchDetailDto : MatchDto
    {
        public int Attendance { get; set; }
        public string Weather { get; set; } = string.Empty;
        public int MinutesPlayed { get; set; }
        public List<MatchPerformanceDto> PlayerPerformances { get; set; } = new List<MatchPerformanceDto>();
        public bool HasAnalysis { get; set; }
        public bool AnalysisCompleted { get; set; }
    }

    public class CreateMatchDto
    {
        [Required]
        public string HomeTeamId { get; set; } = string.Empty;

        [Required]
        public string AwayTeamId { get; set; } = string.Empty;

        public DateTime MatchDate { get; set; }

        [StringLength(100)]
        public string Competition { get; set; } = string.Empty;

        [StringLength(50)]
        public string Season { get; set; } = string.Empty;

        [StringLength(100)]
        public string Venue { get; set; } = string.Empty;

        [StringLength(50)]
        public string Referee { get; set; } = string.Empty;

        public string? TournamentId { get; set; }

        [StringLength(50)]
        public string Round { get; set; } = string.Empty;
    }

    public class UpdateMatchScoreDto
    {
        [Range(0, 50)]
        public int HomeTeamScore { get; set; }

        [Range(0, 50)]
        public int AwayTeamScore { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Finished";

        public int MinutesPlayed { get; set; } = 90;
    }

    public class UpdateMatchStatusDto
    {
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }

    public class MatchPerformanceDto
    {
        public string PlayerId { get; set; } = string.Empty;
        public string MatchId { get; set; } = string.Empty;
        public int MinutesPlayed { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
    }
}