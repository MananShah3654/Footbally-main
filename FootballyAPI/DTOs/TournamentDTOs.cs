using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.DTOs
{
    public class TournamentDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Season { get; set; } = string.Empty;
        public int MaxTeams { get; set; }
        public string Organizer { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public decimal PrizePool { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int TeamsCount { get; set; }
        public int MatchesCount { get; set; }
    }

    public class TournamentDetailDto : TournamentDto
    {
        public string Rules { get; set; } = string.Empty;
        public List<TournamentTeamDto> Teams { get; set; } = new List<TournamentTeamDto>();
        public List<MatchDto> Matches { get; set; } = new List<MatchDto>();
    }

    public class CreateTournamentDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(20)]
        public string Format { get; set; } = "League";

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [StringLength(50)]
        public string Season { get; set; } = string.Empty;

        [Range(2, 64)]
        public int MaxTeams { get; set; } = 16;

        [StringLength(100)]
        public string Organizer { get; set; } = string.Empty;

        [StringLength(500)]
        public string LogoUrl { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Rules { get; set; } = string.Empty;

        public decimal PrizePool { get; set; } = 0;

        [StringLength(50)]
        public string Currency { get; set; } = "USD";
    }

    public class TournamentTeamDto
    {
        public string TeamId { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public int Points { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; }
    }

    public class AddTeamToTournamentDto
    {
        [StringLength(20)]
        public string? Group { get; set; }
    }

    public class UpdateTournamentStatusDto
    {
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}