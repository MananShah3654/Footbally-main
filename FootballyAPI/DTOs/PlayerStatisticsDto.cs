using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.DTOs
{
    public class PlayerStatisticsDto
    {
        public string Id { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public string Season { get; set; } = string.Empty;
        
        // Basic Statistics
        public int GamesPlayed { get; set; }
        public int GamesStarted { get; set; }
        public int MinutesPlayed { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        
        // Defensive Statistics
        public int Tackles { get; set; }
        public int Interceptions { get; set; }
        public int Clearances { get; set; }
        public int BlockedShots { get; set; }
        public int Fouls { get; set; }
        public int FoulsDrawn { get; set; }
        
        // Offensive Statistics
        public int ShotsTotal { get; set; }
        public int ShotsOnTarget { get; set; }
        public double ShotAccuracy { get; set; }
        public int BigChancesCreated { get; set; }
        public int BigChancesMissed { get; set; }
        
        // Passing Statistics
        public int PassesAttempted { get; set; }
        public int PassesCompleted { get; set; }
        public double PassAccuracy { get; set; }
        public int KeyPasses { get; set; }
        public int CrossesAttempted { get; set; }
        public int CrossesCompleted { get; set; }
        
        // Performance Metrics
        public double DistanceCovered { get; set; }
        public int SprintsCompleted { get; set; }
        public double AverageRating { get; set; }
        
        // Team Performance
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public double WinPercentage { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class CreatePlayerStatisticsDto
    {
        [Required]
        public string PlayerId { get; set; } = string.Empty;
        
        [Required]
        public string Season { get; set; } = string.Empty;
        
        public int GamesPlayed { get; set; } = 0;
        public int GamesStarted { get; set; } = 0;
        public int MinutesPlayed { get; set; } = 0;
        public int Goals { get; set; } = 0;
        public int Assists { get; set; } = 0;
        public int YellowCards { get; set; } = 0;
        public int RedCards { get; set; } = 0;
        public int Tackles { get; set; } = 0;
        public int Interceptions { get; set; } = 0;
        public int Clearances { get; set; } = 0;
        public int BlockedShots { get; set; } = 0;
        public int Fouls { get; set; } = 0;
        public int FoulsDrawn { get; set; } = 0;
        public int ShotsTotal { get; set; } = 0;
        public int ShotsOnTarget { get; set; } = 0;
        public int BigChancesCreated { get; set; } = 0;
        public int BigChancesMissed { get; set; } = 0;
        public int PassesAttempted { get; set; } = 0;
        public int PassesCompleted { get; set; } = 0;
        public int KeyPasses { get; set; } = 0;
        public int CrossesAttempted { get; set; } = 0;
        public int CrossesCompleted { get; set; } = 0;
        public double DistanceCovered { get; set; } = 0;
        public int SprintsCompleted { get; set; } = 0;
        public double AverageRating { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Draws { get; set; } = 0;
        public int Losses { get; set; } = 0;
    }
}