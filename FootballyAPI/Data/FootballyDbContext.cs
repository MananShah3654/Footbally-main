using Microsoft.EntityFrameworkCore;
using FootballyAPI.Models;

namespace FootballyAPI.Data
{
    public class FootballyDbContext : DbContext
    {
        public FootballyDbContext(DbContextOptions<FootballyDbContext> options) : base(options)
        {
        }
        
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStatistics> PlayerStatistics { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchPerformance> MatchPerformances { get; set; }
        public DbSet<StatusCheck> StatusChecks { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Player relationships
            modelBuilder.Entity<Player>()
                .HasMany(p => p.Statistics)
                .WithOne(s => s.Player)
                .HasForeignKey(s => s.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<Player>()
                .HasMany(p => p.MatchPerformances)
                .WithOne(mp => mp.Player)
                .HasForeignKey(mp => mp.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Team relationships
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Players)
                .WithOne()
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<Team>()
                .HasMany(t => t.HomeMatches)
                .WithOne(m => m.HomeTeam)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Team>()
                .HasMany(t => t.AwayMatches)
                .WithOne(m => m.AwayTeam)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Match relationships
            modelBuilder.Entity<Match>()
                .HasMany(m => m.PlayerPerformances)
                .WithOne(mp => mp.Match)
                .HasForeignKey(mp => mp.MatchId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes for better performance
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.Name);
                
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.TeamId);
                
            modelBuilder.Entity<PlayerStatistics>()
                .HasIndex(ps => new { ps.PlayerId, ps.Season });
                
            modelBuilder.Entity<Team>()
                .HasIndex(t => t.Name);
                
            modelBuilder.Entity<Match>()
                .HasIndex(m => m.MatchDate);
                
            modelBuilder.Entity<MatchPerformance>()
                .HasIndex(mp => new { mp.PlayerId, mp.MatchId });
        }
    }
}