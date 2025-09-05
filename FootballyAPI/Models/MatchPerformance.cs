using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.Models
{
    public class MatchPerformance
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string MatchId { get; set; } = string.Empty;

        [Required]
        public string PlayerId { get; set; } = string.Empty;

        public int MinutesPlayed { get; set; } = 0;
        public bool Started { get; set; } = false;

        // Match statistics
        public int Goals { get; set; } = 0;
        public int Assists { get; set; } = 0;
        public int Shots { get; set; } = 0;
        public int ShotsOnTarget { get; set; } = 0;
        public int Passes { get; set; } = 0;
        public int PassesCompleted { get; set; } = 0;
        public int KeyPasses { get; set; } = 0;
        public int Tackles { get; set; } = 0;
        public int TacklesWon { get; set; } = 0;
        public int Interceptions { get; set; } = 0;
        public int Clearances { get; set; } = 0;
        public int Crosses { get; set; } = 0;
        public int CrossesCompleted { get; set; } = 0;
        public int Dribbles { get; set; } = 0;
        public int DribblesCompleted { get; set; } = 0;
        public int Fouls { get; set; } = 0;
        public int FoulsDrawn { get; set; } = 0;
        public int Offsides { get; set; } = 0;

        // Cards
        public int YellowCards { get; set; } = 0;
        public int RedCards { get; set; } = 0;

        // Specific events timing
        [StringLength(500)]
        public string GoalMinutes { get; set; } = string.Empty; // e.g., "23, 67, 89"

        [StringLength(500)]
        public string AssistMinutes { get; set; } = string.Empty;

        [StringLength(500)]
        public string CardMinutes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}