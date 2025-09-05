using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.Models
{
    public class PlayerStatistics
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string PlayerId { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Season { get; set; } = string.Empty;

        public int GamesPlayed { get; set; } = 0;
        public int GamesStarted { get; set; } = 0;
        public int MinutesPlayed { get; set; } = 0;

        // Attacking stats
        public int Goals { get; set; } = 0;
        public int Assists { get; set; } = 0;
        public int ShotsTotal { get; set; } = 0;
        public int ShotsOnTarget { get; set; } = 0;
        public int KeyPasses { get; set; } = 0;

        // Defensive stats
        public int Tackles { get; set; } = 0;
        public int Interceptions { get; set; } = 0;
        public int Clearances { get; set; } = 0;
        public int BlockedShots { get; set; } = 0;

        // Passing stats
        public int PassesAttempted { get; set; } = 0;
        public int PassesCompleted { get; set; } = 0;
        public double PassAccuracy => PassesAttempted > 0 ? (double)PassesCompleted / PassesAttempted * 100 : 0;

        // Disciplinary
        public int YellowCards { get; set; } = 0;
        public int RedCards { get; set; } = 0;

        // Overall performance
        public double AverageRating { get; set; } = 0.0;
        public int Wins { get; set; } = 0;
        public int Draws { get; set; } = 0;
        public int Losses { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}