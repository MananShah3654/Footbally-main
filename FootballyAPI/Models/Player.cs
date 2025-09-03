using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.Models
{
    public class Player
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Position { get; set; } = string.Empty;
        
        [Range(16, 50)]
        public int Age { get; set; }
        
        [StringLength(50)]
        public string Nationality { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string TeamId { get; set; } = string.Empty;
        
        [Range(0, 300)]
        public int Height { get; set; } // in cm
        
        [Range(0, 150)]
        public int Weight { get; set; } // in kg
        
        [StringLength(50)]
        public string PreferredFoot { get; set; } = "Right";
        
        [Range(0, 100)]
        public int JerseyNumber { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<PlayerStatistics> Statistics { get; set; } = new List<PlayerStatistics>();
        public virtual ICollection<MatchPerformance> MatchPerformances { get; set; } = new List<MatchPerformance>();
    }
}