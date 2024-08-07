﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TimeHacker.Migrations.Factory;

#nullable disable

namespace TimeHacker.Migrations.Migrations
{
    [DbContext(typeof(MigrationsDbContext))]
    partial class MigrationsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TimeHacker.Domain.Contracts.Entities.Categories.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Color")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(516)
                        .HasColumnType("nvarchar(516)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("UserId")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("TimeHacker.Domain.Contracts.Entities.Categories.CategoryDynamicTask", b =>
                {
                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("DynamicTaskId")
                        .HasColumnType("int");

                    b.HasKey("CategoryId", "DynamicTaskId");

                    b.ToTable("CategoryDynamicTask");
                });

            modelBuilder.Entity("TimeHacker.Domain.Contracts.Entities.Categories.CategoryFixedTask", b =>
                {
                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("FixedTaskId")
                        .HasColumnType("int");

                    b.HasKey("CategoryId", "FixedTaskId");

                    b.ToTable("CategoryFixedTask");
                });

            modelBuilder.Entity("TimeHacker.Domain.Contracts.Entities.Tasks.DynamicTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(516)
                        .HasColumnType("nvarchar(516)");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<TimeSpan>("MaxTimeToFinish")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("MinTimeToFinish")
                        .HasColumnType("time");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<TimeSpan?>("OptimalTimeToFinish")
                        .HasColumnType("time");

                    b.Property<long>("Priority")
                        .HasColumnType("bigint");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTimestamp");

                    b.HasIndex("IsCompleted");

                    b.HasIndex("UserId");

                    b.ToTable("DynamicTask");
                });

            modelBuilder.Entity("TimeHacker.Domain.Contracts.Entities.Tasks.FixedTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(516)
                        .HasColumnType("nvarchar(516)");

                    b.Property<DateTime>("EndTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<long>("Priority")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("StartTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTimestamp");

                    b.HasIndex("IsCompleted");

                    b.HasIndex("StartTimestamp");

                    b.HasIndex("UserId");

                    b.ToTable("FixedTask");
                });

            modelBuilder.Entity("TimeHacker.Domain.Contracts.Entities.Categories.CategoryDynamicTask", b =>
                {
                    b.HasOne("TimeHacker.Domain.Contracts.Entities.Categories.Category", "Category")
                        .WithMany("CategoryDynamicTasks")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("TimeHacker.Domain.Contracts.Entities.Tasks.DynamicTask", "DynamicTask")
                        .WithMany("CategoryDynamicTasks")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("DynamicTask");
                });

            modelBuilder.Entity("TimeHacker.Domain.Contracts.Entities.Categories.CategoryFixedTask", b =>
                {
                    b.HasOne("TimeHacker.Domain.Contracts.Entities.Categories.Category", "Category")
                        .WithMany("CategoryFixedTasks")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("TimeHacker.Domain.Contracts.Entities.Tasks.FixedTask", "FixedTask")
                        .WithMany("CategoryFixedTasks")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("FixedTask");
                });

            modelBuilder.Entity("TimeHacker.Domain.Contracts.Entities.Categories.Category", b =>
                {
                    b.Navigation("CategoryDynamicTasks");

                    b.Navigation("CategoryFixedTasks");
                });

            modelBuilder.Entity("TimeHacker.Domain.Contracts.Entities.Tasks.DynamicTask", b =>
                {
                    b.Navigation("CategoryDynamicTasks");
                });

            modelBuilder.Entity("TimeHacker.Domain.Contracts.Entities.Tasks.FixedTask", b =>
                {
                    b.Navigation("CategoryFixedTasks");
                });
#pragma warning restore 612, 618
        }
    }
}
