using Istu.Navigation.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Infrastructure.EF;

public class BuildingsDbContext : DbContext
{
    public DbSet<BuildingEntity> Buildings { get; set; }

    public DbSet<BuildingObjectEntity> Objects { get; set; }

    public DbSet<EdgeEntity> Edges { get; set; }

    public DbSet<ImageLinkEntity> ImageLinks { get; set; }

    public DbSet<FloorEntity> Floors { get; set; }

#pragma warning disable CS8618 // Required by Entity Framework
    public BuildingsDbContext(DbContextOptions<BuildingsDbContext> options) : base(options)
    {
    }

    public BuildingsDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureBuildings(modelBuilder);
        ConfigureBuildingObjects(modelBuilder);
        ConfigureEdges(modelBuilder);
        ConfigureImageLinks(modelBuilder);
        ConfigureFloors(modelBuilder);
    }

    private void ConfigureImageLinks(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImageLinkEntity>(a =>
        {
            a.Property(x => x.Link).IsRequired();
            a.Property(x => x.ObjectId).IsRequired();
            a.Property(x => x.CreatedByAdmin).IsRequired();
        });
    }

    private void ConfigureBuildings(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BuildingEntity>(a =>
        {
            a.Property(x => x.Title).HasMaxLength(100).IsRequired();
            a.Property(x => x.Description).HasMaxLength(1000);
        });
    }

    private void ConfigureBuildingObjects(ModelBuilder modelBuilder)
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

    private void ConfigureEdges(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EdgeEntity>(a => { a.Property(x => x.FloorNumber).IsRequired(); });

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

    private void ConfigureFloors(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FloorEntity>(a => { a.Property(x => x.FloorNumber).IsRequired(); });

        modelBuilder.Entity<FloorEntity>()
            .HasOne<BuildingEntity>()
            .WithMany()
            .HasForeignKey(x => x.BuildingId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}