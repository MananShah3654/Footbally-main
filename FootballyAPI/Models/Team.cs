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

        [StringLength(20)]
        public string PrimaryColor { get; set; } = string.Empty;

        [StringLength(20)]
        public string SecondaryColor { get; set; } = string.Empty;

        [StringLength(100)]
        public string Manager { get; set; } = string.Empty;

        [StringLength(50)]
        public string League { get; set; } = string.Empty;

        [StringLength(500)]
        public string LogoUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Player> Players { get; set; } = new List<Player>();
        public virtual ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();
    }
}