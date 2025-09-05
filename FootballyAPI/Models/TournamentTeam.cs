using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.Models
{
    public class TournamentTeam
    {
        [Required]
        public string TournamentId { get; set; } = string.Empty;

        [Required]
        public string TeamId { get; set; } = string.Empty;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        [StringLength(20)]
        public string Group { get; set; } = string.Empty; // For group stage tournaments

        public int Points { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Draws { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int GoalsFor { get; set; } = 0;
        public int GoalsAgainst { get; set; } = 0;
        public int GoalDifference { get; set; } = 0;

        // Navigation properties
        public virtual Tournament Tournament { get; set; } = null!;
        public virtual Team Team { get; set; } = null!;
    }
}