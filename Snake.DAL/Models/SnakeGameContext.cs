using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Snake.DAL.Models;

public partial class SnakeGameContext : DbContext
{
    public SnakeGameContext()
    {
    }

    public SnakeGameContext(DbContextOptions<SnakeGameContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Leaderboard> Leaderboards { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString());


    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnectionStringDB"];

        return strConn;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Leaderboard>(entity =>
        {
            entity.HasKey(e => e.ScoreId).HasName("PK__leaderbo__8CA1905053BCE5CF");

            entity.ToTable("leaderboard");

            entity.Property(e => e.ScoreId).HasColumnName("score_id");
            entity.Property(e => e.AchievedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("achieved_at");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Leaderboards)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__leaderboa__user___164452B1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F6C4BDFB2");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E616440D61323").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__users__F3DBC5725024F4EA").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
