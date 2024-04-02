using Istu.Navigation.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Infrastructure.EF;

public class BuildingsDbContext : DbContext
{ 
    public DbSet<BuildingEntity> Buildings { get; set; }

    public DbSet<FloorEntity> Floors { get; set; }

    public DbSet<BuildingObjectEntity> Objects { get; set; }

    public DbSet<EdgeEntity> Edges { get; set; }

    public DbSet<ImageLinkEntity> ImageLinks { get; set; }
    
    #pragma warning disable CS8618 // Required by Entity Framework
    public BuildingsDbContext(DbContextOptions<BuildingsDbContext> options) : base(options)
    {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureBuildingDto(modelBuilder);
        ConfigureFloorDto(modelBuilder);
        ConfigureBuildingObjectDto(modelBuilder);
        ConfigureEdgeDto(modelBuilder);
    }

    private void ConfigureBuildingDto(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BuildingEntity>(a =>
        {
            a.Property(x => x.Title).HasMaxLength(100).IsRequired();
            a.Property(x => x.Description).HasMaxLength(1000);
            a.Property(x => x.FloorNumbers).IsRequired();
        });
    }

    private void ConfigureFloorDto(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FloorEntity>(a =>
        {
            a.Property(x => x.FloorNumber).IsRequired();
        });
        
        modelBuilder.Entity<FloorEntity>()
            .HasKey(f => new { f.BuildingId, f.FloorNumber });

        modelBuilder.Entity<FloorEntity>()
            .HasOne<BuildingObjectEntity>()
            .WithMany()
            .HasForeignKey(x => x.BuildingId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FloorEntity>()
            .HasOne<ImageLinkEntity>()
            .WithOne()
            .HasForeignKey<FloorEntity>(x => x.ImageId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureBuildingObjectDto(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BuildingObjectEntity>(a =>
        {
            a.Property(x => x.Title).HasMaxLength(100).IsRequired();
            a.Property(x => x.Description).HasMaxLength(1000);
            a.Property(x => x.Floor).IsRequired();
            a.Property(x => x.Type).IsRequired();
            a.Property(x => x.X).IsRequired();
            a.Property(x => x.Y).IsRequired();
        });

        modelBuilder.Entity<BuildingObjectEntity>()
            .HasOne<BuildingEntity>()
            .WithMany()
            .HasForeignKey(x => x.BuildingId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureEdgeDto(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EdgeEntity>(a =>
        {
            a.Property(x => x.FloorNumber).IsRequired();
        });
        
        modelBuilder.Entity<EdgeEntity>()
            .HasOne<BuildingObjectEntity>()
            .WithMany()
            .HasForeignKey(x => x.ToObject)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EdgeEntity>()
            .HasOne<BuildingObjectEntity>()
            .WithMany()
            .HasForeignKey(x => x.ToObject)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}