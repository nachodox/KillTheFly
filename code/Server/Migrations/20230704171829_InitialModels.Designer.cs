﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KillTheFly.Server.Migrations
{
    [DbContext(typeof(KTFDatabaseContext))]
    [Migration("20230704171829_InitialModels")]
    partial class InitialModels
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-preview.5.23280.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("KillTheFly.Server.Models.Access", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("EntityGuid")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Guid");

                    b.HasIndex("EntityGuid");

                    b.ToTable("Access");
                });

            modelBuilder.Entity("KillTheFly.Server.Models.GameEntity", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<char>("Avatar")
                        .HasColumnType("character(1)");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("GuidClient")
                        .HasColumnType("text");

                    b.Property<bool>("IsPlayer")
                        .HasColumnType("boolean");

                    b.Property<int>("StartX")
                        .HasColumnType("integer");

                    b.Property<int>("StartY")
                        .HasColumnType("integer");

                    b.Property<int>("X")
                        .HasColumnType("integer");

                    b.Property<int>("Y")
                        .HasColumnType("integer");

                    b.HasKey("Guid");

                    b.ToTable("Entities");
                });

            modelBuilder.Entity("KillTheFly.Server.Models.Kill", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("GameEntityId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MovementId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Guid");

                    b.HasIndex("GameEntityId")
                        .IsUnique();

                    b.HasIndex("MovementId")
                        .IsUnique();

                    b.ToTable("Kills");
                });

            modelBuilder.Entity("KillTheFly.Server.Models.Movement", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Direction")
                        .HasColumnType("integer");

                    b.Property<int>("EndX")
                        .HasColumnType("integer");

                    b.Property<int>("EndY")
                        .HasColumnType("integer");

                    b.Property<Guid>("EntityGuid")
                        .HasColumnType("uuid");

                    b.Property<int>("StartX")
                        .HasColumnType("integer");

                    b.Property<int>("StartY")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Guid");

                    b.HasIndex("EntityGuid");

                    b.ToTable("Movements");
                });

            modelBuilder.Entity("KillTheFly.Server.Models.Access", b =>
                {
                    b.HasOne("KillTheFly.Server.Models.GameEntity", "Entity")
                        .WithMany("Accesses")
                        .HasForeignKey("EntityGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("KillTheFly.Server.Models.Kill", b =>
                {
                    b.HasOne("KillTheFly.Server.Models.GameEntity", "Victim")
                        .WithOne("Kill")
                        .HasForeignKey("KillTheFly.Server.Models.Kill", "GameEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KillTheFly.Server.Models.Movement", "KillerMovement")
                        .WithOne("Kill")
                        .HasForeignKey("KillTheFly.Server.Models.Kill", "MovementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("KillerMovement");

                    b.Navigation("Victim");
                });

            modelBuilder.Entity("KillTheFly.Server.Models.Movement", b =>
                {
                    b.HasOne("KillTheFly.Server.Models.GameEntity", "Entity")
                        .WithMany("Movements")
                        .HasForeignKey("EntityGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("KillTheFly.Server.Models.GameEntity", b =>
                {
                    b.Navigation("Accesses");

                    b.Navigation("Kill");

                    b.Navigation("Movements");
                });

            modelBuilder.Entity("KillTheFly.Server.Models.Movement", b =>
                {
                    b.Navigation("Kill");
                });
#pragma warning restore 612, 618
        }
    }
}
