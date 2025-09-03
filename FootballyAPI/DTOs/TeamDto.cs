using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.DTOs
{
    public class TeamDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Stadium { get; set; } = string.Empty;
        public int Founded { get; set; }
        public string PrimaryColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public string League { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class CreateTeamDto
    {
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
    }
}