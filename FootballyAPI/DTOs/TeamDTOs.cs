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
        public string Manager { get; set; } = string.Empty;
        public string League { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
    }

    public class TeamDetailDto : TeamDto
    {
        public List<PlayerDto> Players { get; set; } = new List<PlayerDto>();
        public List<TournamentDto> Tournaments { get; set; } = new List<TournamentDto>();
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
    }

    public class UpdateTeamDto : CreateTeamDto
    {
    }
}