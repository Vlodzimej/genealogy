﻿// <auto-generated />
using System;
using Genealogy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Genealogy.Migrations
{
    [DbContext(typeof(GenealogyContext))]
    partial class GenealogyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Genealogy.Models.Cemetery", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Location");

                    b.Property<string>("Name");

                    b.Property<bool>("isRemoved");

                    b.HasKey("Id");

                    b.ToTable("Cemeteries");
                });

            modelBuilder.Entity("Genealogy.Models.Link", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Caption");

                    b.Property<int>("Order");

                    b.Property<Guid>("PageId");

                    b.Property<Guid>("TargetPageId");

                    b.HasKey("Id");

                    b.ToTable("Links");
                });

            modelBuilder.Entity("Genealogy.Models.Page", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<string>("Name");

                    b.Property<string>("Title");

                    b.Property<bool>("isRemoved");

                    b.Property<bool>("isSection");

                    b.HasKey("Id");

                    b.ToTable("Pages");
                });

            modelBuilder.Entity("Genealogy.Models.Person", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("CemeteryId");

                    b.Property<string>("FinishDate");

                    b.Property<string>("Firstname");

                    b.Property<string>("Lastname");

                    b.Property<string>("Patronymic");

                    b.Property<string>("Source");

                    b.Property<string>("StartDate");

                    b.Property<bool>("isRemoved");

                    b.HasKey("Id");

                    b.HasIndex("CemeteryId");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("Genealogy.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Genealogy.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("FinishDate");

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsConfirmed");

                    b.Property<string>("LastName");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<Guid?>("RoleId");

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Genealogy.Models.Person", b =>
                {
                    b.HasOne("Genealogy.Models.Cemetery", "Cemetery")
                        .WithMany()
                        .HasForeignKey("CemeteryId");
                });

            modelBuilder.Entity("Genealogy.Models.User", b =>
                {
                    b.HasOne("Genealogy.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");
                });
#pragma warning restore 612, 618
        }
    }
}
