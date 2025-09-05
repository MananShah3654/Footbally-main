using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FootballyAPI.Models;

namespace FootballyAPI.Data
{
    public class FootballyDbContext : IdentityDbContext<IdentityUser>
    {
        public FootballyDbContext(DbContextOptions<FootballyDbContext> options) : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentTeam> TournamentTeams { get; set; }
        public DbSet<PlayerStatistics> PlayerStatistics { get; set; }
        public DbSet<MatchPerformance> MatchPerformances { get; set; }
        public DbSet<PlayerRating> PlayerRatings { get; set; }
        public DbSet<MatchAnalysis> MatchAnalyses { get; set; }
        public DbSet<StatusCheck> StatusChecks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Team relationships
            modelBuilder.Entity<Player>()
                .HasOne<Team>()
                .WithMany()
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Match relationships
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany()
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany()
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Tournament relationships
            modelBuilder.Entity<TournamentTeam>()
                .HasKey(tt => new { tt.TournamentId, tt.TeamId });

            modelBuilder.Entity<TournamentTeam>()
                .HasOne(tt => tt.Tournament)
                .WithMany(t => t.TournamentTeams)
                .HasForeignKey(tt => tt.TournamentId);

            modelBuilder.Entity<TournamentTeam>()
                .HasOne(tt => tt.Team)
                .WithMany()
                .HasForeignKey(tt => tt.TeamId);

            // Configure Match-Tournament relationship
            modelBuilder.Entity<Match>()
                .HasOne<Tournament>()
                .WithMany(t => t.Matches)
                .HasForeignKey(m => m.TournamentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure MatchPerformance relationships
            modelBuilder.Entity<MatchPerformance>()
                .HasOne<Match>()
                .WithMany(m => m.PlayerPerformances)
                .HasForeignKey(mp => mp.MatchId);

            modelBuilder.Entity<MatchPerformance>()
                .HasOne<Player>()
                .WithMany(p => p.MatchPerformances)
                .HasForeignKey(mp => mp.PlayerId);

            // Configure PlayerRating relationships
            modelBuilder.Entity<PlayerRating>()
                .HasOne<Match>()
                .WithMany()
                .HasForeignKey(pr => pr.MatchId);

            modelBuilder.Entity<PlayerRating>()
                .HasOne<Player>()
                .WithMany()
                .HasForeignKey(pr => pr.PlayerId);

            // Configure MatchAnalysis relationships
            modelBuilder.Entity<MatchAnalysis>()
                .HasOne<Match>()
                .WithOne()
                .HasForeignKey<MatchAnalysis>(ma => ma.MatchId);

            modelBuilder.Entity<MatchAnalysis>()
                .HasOne<Player>()
                .WithMany()
                .HasForeignKey(ma => ma.ManOfTheMatchId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure PlayerStatistics relationships
            modelBuilder.Entity<PlayerStatistics>()
                .HasOne<Player>()
                .WithMany(p => p.Statistics)
                .HasForeignKey(ps => ps.PlayerId);
        }
    }
}