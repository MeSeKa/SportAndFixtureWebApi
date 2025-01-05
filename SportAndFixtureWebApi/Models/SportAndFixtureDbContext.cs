using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SportAndFixtureWebApi.Models;

public partial class SportAndFixtureDbContext : DbContext
{
    public SportAndFixtureDbContext()
    {
    }

    public SportAndFixtureDbContext(DbContextOptions<SportAndFixtureDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Fixture> Fixtures { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=MESEKAPC\\SQLEXPRESS04;Initial Catalog=SportAndFixtureDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fixture>(entity =>
        {
            entity.Property(e => e.Id)
				.ValueGeneratedOnAdd(); // ID otomatik artış
			entity.Property(e => e.AwayTeamId).HasColumnName("awayTeamId");
            entity.Property(e => e.AwayTeamScore).HasColumnName("awayTeamScore");
            entity.Property(e => e.HomeTeamId).HasColumnName("homeTeamId");
            entity.Property(e => e.HomeTeamScore).HasColumnName("homeTeamScore");
            entity.Property(e => e.MatchDate).HasColumnName("matchDate");

            entity.HasOne(d => d.AwayTeam).WithMany(p => p.FixtureAwayTeams)
                .HasForeignKey(d => d.AwayTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AwayTeam_Teams");

            entity.HasOne(d => d.HomeTeam).WithMany(p => p.FixtureHomeTeams)
                .HasForeignKey(d => d.HomeTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HomeTeam_Teams");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("Player");

            entity.Property(e => e.Id)
				.ValueGeneratedOnAdd(); // ID otomatik artış
			entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("fullName");
            entity.Property(e => e.Position)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("position");
            entity.Property(e => e.TeamId).HasColumnName("teamId");

            entity.HasOne(d => d.Team).WithMany(p => p.Players)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_Player_Teams");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(e => e.Id)
				.ValueGeneratedOnAdd(); // ID otomatik artış
			entity.Property(e => e.City)
                .HasMaxLength(25)
                .IsFixedLength()
                .HasColumnName("city");
            entity.Property(e => e.TeamName)
                .HasMaxLength(25)
                .IsFixedLength()
                .HasColumnName("teamName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
