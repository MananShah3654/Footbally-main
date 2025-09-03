using System.ComponentModel.DataAnnotations;

namespace FootballyAPI.Models
{
    public class StatusCheck
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string ClientName { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Status { get; set; } = "Active";
    }
}