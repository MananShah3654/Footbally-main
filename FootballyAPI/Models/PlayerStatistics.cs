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
        public string Season { get; set; } = string.Empty;
        
        // Basic Statistics
        public int GamesPlayed { get; set; } = 0;
        public int GamesStarted { get; set; } = 0;
        public int MinutesPlayed { get; set; } = 0;
        public int Goals { get; set; } = 0;
        public int Assists { get; set; } = 0;
        public int YellowCards { get; set; } = 0;
        public int RedCards { get; set; } = 0;
        
        // Defensive Statistics
        public int Tackles { get; set; } = 0;
        public int Interceptions { get; set; } = 0;
        public int Clearances { get; set; } = 0;
        public int BlockedShots { get; set; } = 0;
        public int Fouls { get; set; } = 0;
        public int FoulsDrawn { get; set; } = 0;
        
        // Offensive Statistics
        public int ShotsTotal { get; set; } = 0;
        public int ShotsOnTarget { get; set; } = 0;
        public double ShotAccuracy => ShotsTotal > 0 ? (double)ShotsOnTarget / ShotsTotal * 100 : 0;
        public int BigChancesCreated { get; set; } = 0;
        public int BigChancesMissed { get; set; } = 0;
        
        // Passing Statistics
        public int PassesAttempted { get; set; } = 0;
        public int PassesCompleted { get; set; } = 0;
        public double PassAccuracy => PassesAttempted > 0 ? (double)PassesCompleted / PassesAttempted * 100 : 0;
        public int KeyPasses { get; set; } = 0;
        public int CrossesAttempted { get; set; } = 0;
        public int CrossesCompleted { get; set; } = 0;
        
        // Performance Metrics
        public double DistanceCovered { get; set; } = 0; // in km
        public int SprintsCompleted { get; set; } = 0;
        public double AverageRating { get; set; } = 0;
        
        // Team Performance
        public int Wins { get; set; } = 0;
        public int Draws { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public double WinPercentage => GamesPlayed > 0 ? (double)Wins / GamesPlayed * 100 : 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public virtual Player Player { get; set; } = null!;
    }
}