using Microsoft.EntityFrameworkCore;

using TAS.Core.Models;

namespace TAS.Core.Repositories
{
    public class RateDbContext : DbContext
    {
        public DbSet<Rate> Rates { get; set; }

        public RateDbContext(DbContextOptions<RateDbContext> options): base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var rateBuilder = builder.Entity<Rate>();

            rateBuilder.HasKey(r => new { r.Title, r.Date });

            rateBuilder.Property(r => r.FullName)
                .HasMaxLength(150)
                .HasColumnName("RateName");

            rateBuilder.Property(r => r.Title)
                .HasMaxLength(3)
                .HasColumnName("ShortName");

            rateBuilder.Property(r => r.Description)
                .HasColumnType("decimal(10,2)")
                .HasColumnName("RateCount");

            rateBuilder.Property(r => r.Date)
                .HasColumnName("RateDate");
        }
    }
}
