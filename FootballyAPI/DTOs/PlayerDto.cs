using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.DTOs
{
    public class PlayerDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public int Height { get; set; }
        public int Weight { get; set; }
        public string PreferredFoot { get; set; } = string.Empty;
        public int JerseyNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class CreatePlayerDto
    {
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
        public int Height { get; set; }
        
        [Range(0, 150)]
        public int Weight { get; set; }
        
        [StringLength(50)]
        public string PreferredFoot { get; set; } = "Right";
        
        [Range(0, 100)]
        public int JerseyNumber { get; set; }
    }
    
    public class UpdatePlayerDto
    {
        [StringLength(100)]
        public string? Name { get; set; }
        
        [StringLength(50)]
        public string? Position { get; set; }
        
        [Range(16, 50)]
        public int? Age { get; set; }
        
        [StringLength(50)]
        public string? Nationality { get; set; }
        
        [StringLength(100)]
        public string? TeamId { get; set; }
        
        [Range(0, 300)]
        public int? Height { get; set; }
        
        [Range(0, 150)]
        public int? Weight { get; set; }
        
        [StringLength(50)]
        public string? PreferredFoot { get; set; }
        
        [Range(0, 100)]
        public int? JerseyNumber { get; set; }
    }
}