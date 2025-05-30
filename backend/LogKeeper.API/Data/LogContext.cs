using Microsoft.EntityFrameworkCore;
using LogKeeper.API.Models;

namespace LogKeeper.API.Data
{
    public class LogContext : DbContext
    {
        public LogContext(DbContextOptions<LogContext> options) : base(options)
        {
        }

        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Level)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Message)
                    .IsRequired();

                entity.Property(e => e.Source)
                    .HasMaxLength(100);

                entity.Property(e => e.UserId)
                    .HasMaxLength(100);

                entity.Property(e => e.Timestamp)
                    .IsRequired();

                // Индексы для оптимизации поиска
                entity.HasIndex(e => e.Level);
                entity.HasIndex(e => e.Source);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.UserId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}