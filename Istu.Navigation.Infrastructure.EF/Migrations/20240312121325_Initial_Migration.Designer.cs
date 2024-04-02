﻿// <auto-generated />
using System;
using Istu.Navigation.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Istu.Navigation.Infrastructure.EF.Migrations
{
    [DbContext(typeof(BuildingsDbContext))]
    [Migration("20240312121325_Initial_Migration")]
    partial class Initial_Migration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Istu.Navigation.Domain.Models.Entities.BuildingEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<int>("FloorNumbers")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.ToTable("Buildings");
                });

            modelBuilder.Entity("Istu.Navigation.Domain.Models.Entities.BuildingObjectEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BuildingId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<int>("Floor")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<double>("X")
                        .HasColumnType("double precision");

                    b.Property<double>("Y")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.ToTable("Objects");
                });

            modelBuilder.Entity("Istu.Navigation.Domain.Models.Entities.EdgeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BuildingId")
                        .HasColumnType("uuid");

                    b.Property<int>("FloorNumber")
                        .HasColumnType("integer");

                    b.Property<Guid>("FromObject")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("ToObject")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ToObject");

                    b.ToTable("Edges");
                });

            modelBuilder.Entity("Istu.Navigation.Domain.Models.Entities.FloorEntity", b =>
                {
                    b.Property<Guid>("BuildingId")
                        .HasColumnType("uuid");

                    b.Property<int>("FloorNumber")
                        .HasColumnType("integer");

                    b.Property<Guid>("ImageId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("BuildingId", "FloorNumber");

                    b.HasIndex("ImageId")
                        .IsUnique();

                    b.ToTable("Floors");
                });

            modelBuilder.Entity("Istu.Navigation.Domain.Models.Entities.ImageLinkEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ObjectId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("ImageLinks");
                });

            modelBuilder.Entity("Istu.Navigation.Domain.Models.Entities.BuildingObjectEntity", b =>
                {
                    b.HasOne("Istu.Navigation.Domain.Models.Entities.BuildingEntity", null)
                        .WithMany()
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Istu.Navigation.Domain.Models.Entities.EdgeEntity", b =>
                {
                    b.HasOne("Istu.Navigation.Domain.Models.Entities.BuildingObjectEntity", null)
                        .WithMany()
                        .HasForeignKey("ToObject")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Istu.Navigation.Domain.Models.Entities.FloorEntity", b =>
                {
                    b.HasOne("Istu.Navigation.Domain.Models.Entities.BuildingObjectEntity", null)
                        .WithMany()
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Istu.Navigation.Domain.Models.Entities.ImageLinkEntity", null)
                        .WithOne()
                        .HasForeignKey("Istu.Navigation.Domain.Models.Entities.FloorEntity", "ImageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
