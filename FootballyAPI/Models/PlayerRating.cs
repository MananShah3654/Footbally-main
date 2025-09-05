using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.Models
{
    public class PlayerRating
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string MatchId { get; set; } = string.Empty;

        [Required]
        public string PlayerId { get; set; } = string.Empty;

        [Required]
        public string RatedBy { get; set; } = string.Empty; // User ID of the referee/rater

        [Range(1.0, 10.0)]
        public double Rating { get; set; } = 6.0;

        // Detailed ratings breakdown
        [Range(1.0, 10.0)]
        public double AttackingRating { get; set; } = 6.0;

        [Range(1.0, 10.0)]
        public double DefendingRating { get; set; } = 6.0;

        [Range(1.0, 10.0)]
        public double PassingRating { get; set; } = 6.0;

        [Range(1.0, 10.0)]
        public double PhysicalRating { get; set; } = 6.0;

        [Range(1.0, 10.0)]
        public double MentalRating { get; set; } = 6.0;

        [StringLength(1000)]
        public string Comments { get; set; } = string.Empty;

        [StringLength(500)]
        public string PositiveHighlights { get; set; } = string.Empty;

        [StringLength(500)]
        public string AreasForImprovement { get; set; } = string.Empty;

        public bool IsManOfTheMatch { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}