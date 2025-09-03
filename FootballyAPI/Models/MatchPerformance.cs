using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.Models
{
    public class MatchPerformance
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string PlayerId { get; set; } = string.Empty;
        
        [Required]
        public string MatchId { get; set; } = string.Empty;
        
        public bool IsStarting { get; set; } = false;
        public int MinutesPlayed { get; set; } = 0;
        
        // Match-specific statistics
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
        
        [Range(0, 10)]
        public double Rating { get; set; } = 0;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Player Player { get; set; } = null!;
        public virtual Match Match { get; set; } = null!;
    }
}