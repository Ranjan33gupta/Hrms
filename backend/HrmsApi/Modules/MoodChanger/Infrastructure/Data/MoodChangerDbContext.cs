using HrmsApi.Modules.MoodChanger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrmsApi.Modules.MoodChanger.Infrastructure.Data
{
    public class MoodChangerDbContext : DbContext
    {
        public MoodChangerDbContext(DbContextOptions<MoodChangerDbContext> options) : base(options)
        {
        }

        public DbSet<MoodEntry> MoodEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MoodEntry>(entity =>
            {
                entity.ToTable("MoodEntries");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserInput).IsRequired();
                entity.Property(e => e.DetectedMood).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ResponseContent).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
