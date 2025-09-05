using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.Models
{
    public class Tournament
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(20)]
        public string Format { get; set; } = "League"; // League, Knockout, Group+Knockout

        [StringLength(20)]
        public string Status { get; set; } = "Upcoming"; // Upcoming, Active, Completed, Cancelled

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [StringLength(50)]
        public string Season { get; set; } = string.Empty;

        [Range(2, 64)]
        public int MaxTeams { get; set; } = 16;

        [StringLength(100)]
        public string Organizer { get; set; } = string.Empty;

        [StringLength(500)]
        public string LogoUrl { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Rules { get; set; } = string.Empty;

        // Prize information
        public decimal PrizePool { get; set; } = 0;

        [StringLength(50)]
        public string Currency { get; set; } = "USD";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();
        public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}