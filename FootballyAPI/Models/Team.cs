using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.Models
{
    public class Team
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string ShortName { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string City { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Country { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Stadium { get; set; } = string.Empty;
        
        public int Founded { get; set; }
        
        [StringLength(7)]
        public string PrimaryColor { get; set; } = "#000000";
        
        [StringLength(7)]
        public string SecondaryColor { get; set; } = "#FFFFFF";
        
        [StringLength(500)]
        public string LogoUrl { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Manager { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string League { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Player> Players { get; set; } = new List<Player>();
        public virtual ICollection<Match> HomeMatches { get; set; } = new List<Match>();
        public virtual ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    }
}