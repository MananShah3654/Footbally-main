using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.Models
{
    public class Match
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string HomeTeamId { get; set; } = string.Empty;
        
        [Required]
        public string AwayTeamId { get; set; } = string.Empty;
        
        public DateTime MatchDate { get; set; }
        
        [StringLength(100)]
        public string Competition { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Season { get; set; } = string.Empty;
        
        [Range(0, 50)]
        public int HomeTeamScore { get; set; } = 0;
        
        [Range(0, 50)]
        public int AwayTeamScore { get; set; } = 0;
        
        [StringLength(20)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Live, Finished, Postponed, Cancelled
        
        [StringLength(100)]
        public string Venue { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Referee { get; set; } = string.Empty;
        
        public int Attendance { get; set; } = 0;
        
        [StringLength(20)]
        public string Weather { get; set; } = string.Empty;
        
        public int MinutesPlayed { get; set; } = 90;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Team HomeTeam { get; set; } = null!;
        public virtual Team AwayTeam { get; set; } = null!;
        public virtual ICollection<MatchPerformance> PlayerPerformances { get; set; } = new List<MatchPerformance>();
    }
}