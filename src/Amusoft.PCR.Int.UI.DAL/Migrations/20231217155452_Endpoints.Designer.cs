﻿// <auto-generated />
using System;
using Amusoft.PCR.Int.UI.DAL.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Amusoft.PCR.Int.UI.DAL.Migrations
{
    [DbContext(typeof(UiDbContext))]
    [Migration("20231217155452_Endpoints")]
    partial class Endpoints
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("Amusoft.PCR.Domain.UI.Entities.BearerToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("EndpointAccountId")
                        .HasColumnType("TEXT");

                    b.Property<long>("Expires")
                        .HasColumnType("INTEGER");

                    b.Property<long>("IssuedAt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EndpointAccountId");

                    b.ToTable("BearerTokens");
                });

            modelBuilder.Entity("Amusoft.PCR.Domain.UI.Entities.Endpoint", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Address")
                        .IsUnique();

                    b.ToTable("Endpoints");
                });

            modelBuilder.Entity("Amusoft.PCR.Domain.UI.Entities.EndpointAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("EndpointId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Email");

                    b.HasIndex("EndpointId", "Email")
                        .IsUnique();

                    b.ToTable("EndpointAccounts");
                });

            modelBuilder.Entity("Amusoft.PCR.Domain.UI.Entities.LogEntry", b =>
                {
                    b.Property<string>("CallSite")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("LogLevel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Logger")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("StackTrace")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("Time")
                        .HasColumnType("INTEGER");

                    b.ToTable("LogEntries");
                });

            modelBuilder.Entity("Amusoft.PCR.Domain.UI.Entities.BearerToken", b =>
                {
                    b.HasOne("Amusoft.PCR.Domain.UI.Entities.EndpointAccount", "EndpointAccount")
                        .WithMany("BearerTokens")
                        .HasForeignKey("EndpointAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EndpointAccount");
                });

            modelBuilder.Entity("Amusoft.PCR.Domain.UI.Entities.EndpointAccount", b =>
                {
                    b.HasOne("Amusoft.PCR.Domain.UI.Entities.Endpoint", "Endpoint")
                        .WithMany("Accounts")
                        .HasForeignKey("EndpointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Endpoint");
                });

            modelBuilder.Entity("Amusoft.PCR.Domain.UI.Entities.Endpoint", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("Amusoft.PCR.Domain.UI.Entities.EndpointAccount", b =>
                {
                    b.Navigation("BearerTokens");
                });
#pragma warning restore 612, 618
        }
    }
}