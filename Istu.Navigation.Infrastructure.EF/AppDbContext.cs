using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Models.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Infrastructure.EF;

public class AppDbContext : DbContext
{
    public DbSet<BuildingEntity> Buildings { get; set; }

    public DbSet<BuildingObjectEntity> Objects { get; set; }

    public DbSet<EdgeEntity> Edges { get; set; }

    public DbSet<ImageInfoEntity> Images { get; set; }

    public DbSet<FloorEntity> Floors { get; set; }
    
    public DbSet<UserEntity> Users { get; set; }

#pragma warning disable CS8618 // Required by Entity Framework
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public AppDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureBuildings(modelBuilder);
        ConfigureBuildingObjects(modelBuilder);
        ConfigureEdges(modelBuilder);
        ConfigureImageLinks(modelBuilder);
        ConfigureFloors(modelBuilder);
        ConfigureUsers(modelBuilder);
    }
    
    private void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(a =>
        {
            a.Property(x => x.Email).HasMaxLength(100).IsRequired();
            a.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            a.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            a.Property(x => x.Role).IsRequired();
        });
        modelBuilder.Entity<UserEntity>().HasIndex(x => x.Email).IsUnique();
    }

    private void ConfigureImageLinks(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImageInfoEntity>(a =>
        {
            a.Property(x => x.ObjectId).IsRequired();
            a.Property(x => x.Title).HasMaxLength(100).IsRequired();
        });
        modelBuilder.Entity<ImageInfoEntity>().HasIndex(x => new { x.ObjectId });
    }

    private void ConfigureBuildings(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BuildingEntity>(a =>
        {
            a.Property(x => x.Title).HasMaxLength(100).IsRequired();
            a.Property(x => x.Description).HasMaxLength(1000);
            a.Property(x => x.Latitude).IsRequired();
            a.Property(x => x.Longitude).IsRequired();
            a.Property(x => x.Address).HasMaxLength(100).IsRequired();
        });
        modelBuilder.Entity<BuildingEntity>().HasIndex(x => x.Title).IsUnique();
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
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<BuildingObjectEntity>().HasIndex(x => new { x.BuildingId });
    }

    private void ConfigureEdges(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EdgeEntity>(a => { a.Property(x => x.FloorNumber).IsRequired(); });

        modelBuilder.Entity<EdgeEntity>()
            .HasOne<BuildingObjectEntity>()
            .WithMany()
            .HasForeignKey(x => x.ToObject)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EdgeEntity>()
            .HasOne<BuildingObjectEntity>()
            .WithMany()
            .HasForeignKey(x => x.ToObject)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EdgeEntity>().HasIndex(x => new { x.BuildingId, x.ToObject, x.FromObject });
    }

    private void ConfigureFloors(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FloorEntity>(a => { a.Property(x => x.FloorNumber).IsRequired(); });

        modelBuilder.Entity<FloorEntity>()
            .HasOne<BuildingEntity>()
            .WithMany()
            .HasForeignKey(x => x.BuildingId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<FloorEntity>().HasIndex(x => new { x.BuildingId });
    }
}