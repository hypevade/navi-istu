using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Infrastructure.EF;

public class BuildingsDbContext: DbContext
{
    public DbSet<BuildingDto> Buildings { get; set; }

    public DbSet<FloorDto> Floors { get; set; }
    
    public DbSet<BuildingObjectDto> Objects { get; set; }
    
    public DbSet<EdgeDto> Edges { get; set; }
    
    public DbSet<ImageLinkDto> ImageLinks { get; set; }

    public BuildingsDbContext(DbContextOptions<BuildingsDbContext> options): base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BuildingDto>(a =>
        {
            a.Property(x => x.Title).HasMaxLength(100).IsRequired();
            a.Property(x => x.Description).HasMaxLength(1000);
            a.Property(x => x.FloorNumbers).IsRequired();
        });
        
    }
}