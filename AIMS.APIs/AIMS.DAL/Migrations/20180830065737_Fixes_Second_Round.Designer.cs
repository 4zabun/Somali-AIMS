﻿// <auto-generated />
using System;
using AIMS.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AIMS.DAL.Migrations
{
    [DbContext(typeof(AIMSDbContext))]
    [Migration("20180830065737_Fixes_Second_Round")]
    partial class Fixes_Second_Round
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AIMS.Models.EFLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Latitude");

                    b.Property<string>("Location");

                    b.Property<int>("Longitude");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("AIMS.Models.EFOrganization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("AIMS.Models.EFProject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<DateTime>("DateEnded");

                    b.Property<DateTime>("DateStarted");

                    b.Property<int>("LocationId");

                    b.Property<string>("Objective");

                    b.Property<int>("SectorId");

                    b.Property<string>("Title")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.HasIndex("SectorId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectCustomFields", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FieldTitle");

                    b.Property<int>("FieldType");

                    b.Property<int?>("ProjectId");

                    b.Property<int?>("SectorId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SectorId");

                    b.ToTable("ProjectCustomFields");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectFunders", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("FunderId");

                    b.HasKey("ProjectId", "FunderId");

                    b.HasIndex("FunderId");

                    b.ToTable("ProjectFunders");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectFundings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(9 ,2)");

                    b.Property<string>("Currency");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(9, 2)");

                    b.Property<int>("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectFundings");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectImplementers", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("ImplementerId");

                    b.HasKey("ProjectId", "ImplementerId");

                    b.HasIndex("ImplementerId");

                    b.ToTable("ProjectImplementers");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectLogs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Dated");

                    b.Property<string>("NewValues");

                    b.Property<string>("OldValues");

                    b.Property<int>("ProjectId");

                    b.Property<int?>("UpdatedById");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UpdatedById");

                    b.ToTable("ProjectLogs");
                });

            modelBuilder.Entity("AIMS.Models.EFSector", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Sectors");
                });

            modelBuilder.Entity("AIMS.Models.EFStaticReports", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("StaticReports");
                });

            modelBuilder.Entity("AIMS.Models.EFTimeIntervals", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DurationInMonths");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("TimeIntervals");
                });

            modelBuilder.Entity("AIMS.Models.EFUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<bool>("IsApproved");

                    b.Property<string>("Name");

                    b.Property<int>("OrganizationId");

                    b.Property<string>("Password");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<int>("UserType");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AIMS.Models.EFUserNotifications", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Dated");

                    b.Property<bool>("IsSeen");

                    b.Property<string>("Message");

                    b.Property<int>("OrganizationId");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("UserNotifications");
                });

            modelBuilder.Entity("AIMS.Models.EFUserSubscriptions", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("ReportId");

                    b.Property<DateTime>("DateSubscribed");

                    b.Property<int>("IntervalId");

                    b.Property<bool>("IsActive");

                    b.Property<DateTime>("ReportSentOn");

                    b.HasKey("UserId", "ReportId");

                    b.HasIndex("IntervalId");

                    b.HasIndex("ReportId");

                    b.ToTable("UserSubscriptions");
                });

            modelBuilder.Entity("AIMS.Models.EFProject", b =>
                {
                    b.HasOne("AIMS.Models.EFLocation", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFSector", "Sector")
                        .WithMany()
                        .HasForeignKey("SectorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectCustomFields", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");

                    b.HasOne("AIMS.Models.EFSector", "Sector")
                        .WithMany()
                        .HasForeignKey("SectorId");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectFunders", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganization", "Funder")
                        .WithMany()
                        .HasForeignKey("FunderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectFundings", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectImplementers", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganization", "Implementer")
                        .WithMany()
                        .HasForeignKey("ImplementerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectLogs", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFUser", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById");
                });

            modelBuilder.Entity("AIMS.Models.EFUser", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFUserNotifications", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFUserSubscriptions", b =>
                {
                    b.HasOne("AIMS.Models.EFTimeIntervals", "TimeInterval")
                        .WithMany()
                        .HasForeignKey("IntervalId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFStaticReports", "Report")
                        .WithMany()
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
