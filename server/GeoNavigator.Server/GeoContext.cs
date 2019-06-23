using GeoNavigator.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoNavigator.Server
{
    public class GeoContext : DbContext
    {
        public GeoContext(DbContextOptions<GeoContext> options) : base(options)
        {
        }

        public GeoContext()
        {
        }
        
        public DbSet<Block> Blocks { get; set; }
        
        public DbSet<VeinInfo> Veins { get; set; }
        public DbSet<VeinDimensionInfo> VeinDimensions { get; set; }
        public DbSet<VeinOreInfo> VeinOres { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<VeinInfo>()
                .HasMany(v => v.Dimensions)
                .WithOne(d => d.VeinInfo)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<VeinInfo>()
                .HasMany(v => v.Ores)
                .WithOne(o => o.VeinInfo)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<VeinInfo>()
                .Property(v => v.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<VeinDimensionInfo>()
                .Property(v => v.Id)
                .ValueGeneratedOnAdd();
            
            builder.Entity<VeinOreInfo>()
                .Property(v => v.Id)
                .ValueGeneratedOnAdd();
            
            builder.Entity<Block>()
                .Property(v => v.Id)
                .ValueGeneratedOnAdd();
        }
    }
}