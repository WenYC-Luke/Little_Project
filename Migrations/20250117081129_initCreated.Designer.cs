﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Farmer_Project.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250117081129_initCreated")]
    partial class initCreated
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Farmer_Project.Models.Entity.FarmersArticles", b =>
                {
                    b.Property<int>("ArticlesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ArticlesId"));

                    b.Property<string>("ArticleImagePath")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ArticleSummary")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ArticleTitle")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ArticleType")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FarmersId")
                        .HasColumnType("int");

                    b.Property<bool>("IsPublished")
                        .HasColumnType("bit");

                    b.HasKey("ArticlesId");

                    b.HasIndex("FarmersId");

                    b.ToTable("FarmersArticles");
                });

            modelBuilder.Entity("Farmer_Project.Models.Entity.FarmersArticlesDetails", b =>
                {
                    b.Property<int>("ArticlesId")
                        .HasColumnType("int");

                    b.Property<int>("DetailId")
                        .HasColumnType("int");

                    b.Property<string>("SubContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubImagePath")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("SubTitle")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("ArticlesId", "DetailId");

                    b.ToTable("FarmersArticlesDetails");
                });

            modelBuilder.Entity("Farmer_Project.Models.Entity.FarmersInfo", b =>
                {
                    b.Property<int>("FarmersId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FarmersId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CropsType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FarmName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("PlantType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("FarmersId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("FarmName")
                        .IsUnique();

                    b.ToTable("FarmersInfo");
                });

            modelBuilder.Entity("Farmer_Project.Models.Entity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("Farmer_Project.Models.Entity.FarmersArticles", b =>
                {
                    b.HasOne("Farmer_Project.Models.Entity.FarmersInfo", "FarmersInfo")
                        .WithMany("FarmersArticles")
                        .HasForeignKey("FarmersId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("FarmersInfo");
                });

            modelBuilder.Entity("Farmer_Project.Models.Entity.FarmersArticlesDetails", b =>
                {
                    b.HasOne("Farmer_Project.Models.Entity.FarmersArticles", "FarmersArticles")
                        .WithMany("FarmersArticlesDetails")
                        .HasForeignKey("ArticlesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FarmersArticles");
                });

            modelBuilder.Entity("Farmer_Project.Models.Entity.FarmersArticles", b =>
                {
                    b.Navigation("FarmersArticlesDetails");
                });

            modelBuilder.Entity("Farmer_Project.Models.Entity.FarmersInfo", b =>
                {
                    b.Navigation("FarmersArticles");
                });
#pragma warning restore 612, 618
        }
    }
}
