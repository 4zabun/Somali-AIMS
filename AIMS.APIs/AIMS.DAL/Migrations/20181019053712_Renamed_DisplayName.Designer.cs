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
    [Migration("20181019053712_Renamed_DisplayName")]
    partial class Renamed_DisplayName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AIMS.Models.EFCustomFields", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ActiveFrom");

                    b.Property<DateTime>("ActiveUpto");

                    b.Property<string>("FieldTitle");

                    b.Property<int>("FieldType");

                    b.HasKey("Id");

                    b.ToTable("CustomFields");
                });

            modelBuilder.Entity("AIMS.Models.EFLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("EFProjectId");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(9, 2)");

                    b.Property<string>("Location");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(9, 2)");

                    b.HasKey("Id");

                    b.HasIndex("EFProjectId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("AIMS.Models.EFLogs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActionPerformed");

                    b.Property<DateTime>("Dated");

                    b.Property<string>("NewValues");

                    b.Property<string>("OldValues");

                    b.Property<int>("ProjectId");

                    b.Property<int?>("UpdatedById");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UpdatedById");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("AIMS.Models.EFOrganization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("OrganizationName");

                    b.Property<int?>("OrganizationTypeId");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationTypeId");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("AIMS.Models.EFOrganizationTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TypeName");

                    b.HasKey("Id");

                    b.ToTable("OrganizationTypes");
                });

            modelBuilder.Entity("AIMS.Models.EFProject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Objective");

                    b.Property<int?>("ProjectTypeId");

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("Title")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("ProjectTypeId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectCustomFields", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("CustomFieldId");

                    b.Property<string>("Value");

                    b.HasKey("ProjectId", "CustomFieldId");

                    b.HasIndex("CustomFieldId");

                    b.ToTable("ProjectCustomFields");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDisbursements", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("StartingYear");

                    b.Property<int>("EndingMonth");

                    b.Property<int>("EndingYear");

                    b.Property<int>("Id");

                    b.Property<decimal>("Percentage")
                        .HasColumnType("decimal(9, 2)");

                    b.Property<int>("StartingMonth");

                    b.HasKey("ProjectId", "StartingYear");

                    b.ToTable("ProjectDisbursements");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDocuments", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DocumentTitle");

                    b.Property<string>("DocumentUrl");

                    b.Property<int>("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectDocuments");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectFunders", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("FunderId");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(9 ,2)");

                    b.Property<string>("Currency");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(9, 2)");

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

                    b.Property<int>("FunderId");

                    b.Property<int>("GrantType");

                    b.Property<int>("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("FunderId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectFundings");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectImplementors", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("ImplementorId");

                    b.HasKey("ProjectId", "ImplementorId");

                    b.HasIndex("ImplementorId");

                    b.ToTable("ProjectImplementors");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectLocations", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("LocationId");

                    b.Property<decimal>("Percentage")
                        .HasColumnType("decimal(9, 2)");

                    b.HasKey("ProjectId", "LocationId");

                    b.HasIndex("LocationId");

                    b.ToTable("ProjectLocations");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectMarkers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Marker");

                    b.Property<decimal>("Percentage")
                        .HasColumnType("decimal(9, 2)");

                    b.Property<int>("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectMarkers");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectSectors", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("SectorId");

                    b.Property<decimal>("ContributedAmount")
                        .HasColumnType("decimal(9, 2)");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(9, 2)");

                    b.HasKey("ProjectId", "SectorId");

                    b.HasIndex("SectorId");

                    b.ToTable("ProjectSectors");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("ProjectTypes");
                });

            modelBuilder.Entity("AIMS.Models.EFReportSubscriptions", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("ReportId");

                    b.HasKey("UserId", "ReportId");

                    b.HasIndex("ReportId");

                    b.ToTable("ReportSubscriptions");
                });

            modelBuilder.Entity("AIMS.Models.EFSector", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CategoryId");

                    b.Property<string>("SectorName");

                    b.Property<int?>("SubCategoryId");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("SubCategoryId");

                    b.ToTable("Sectors");
                });

            modelBuilder.Entity("AIMS.Models.EFSectorCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Category");

                    b.Property<int>("SectorTypeId");

                    b.HasKey("Id");

                    b.HasIndex("SectorTypeId");

                    b.ToTable("SectorCategories");
                });

            modelBuilder.Entity("AIMS.Models.EFSectorMappings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MappedSectorId");

                    b.Property<int>("NativeSectorId");

                    b.Property<int>("SectorTypeId");

                    b.HasKey("Id");

                    b.ToTable("SectorMappings");
                });

            modelBuilder.Entity("AIMS.Models.EFSectorSubCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("SectorCategoryId");

                    b.Property<string>("SubCategory");

                    b.HasKey("Id");

                    b.HasIndex("SectorCategoryId");

                    b.ToTable("SectorSubCategories");
                });

            modelBuilder.Entity("AIMS.Models.EFSectorTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TypeName");

                    b.HasKey("Id");

                    b.ToTable("SectorTypes");
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

            modelBuilder.Entity("AIMS.Models.EFUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ApprovedById");

                    b.Property<DateTime?>("ApprovedOn");

                    b.Property<DateTime?>("DeActivatedOn");

                    b.Property<string>("Email");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsApproved");

                    b.Property<string>("Name");

                    b.Property<int>("OrganizationId");

                    b.Property<string>("Password");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<int>("UserType");

                    b.HasKey("Id");

                    b.HasIndex("ApprovedById");

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

                    b.Property<int>("NotificationType");

                    b.Property<int>("OrganizationId");

                    b.Property<int>("TreatmentId");

                    b.Property<int>("UserType");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("UserNotifications");
                });

            modelBuilder.Entity("AIMS.Models.EFLocation", b =>
                {
                    b.HasOne("AIMS.Models.EFProject")
                        .WithMany("Locations")
                        .HasForeignKey("EFProjectId");
                });

            modelBuilder.Entity("AIMS.Models.EFLogs", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFUser", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById");
                });

            modelBuilder.Entity("AIMS.Models.EFOrganization", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganizationTypes", "OrganizationType")
                        .WithMany()
                        .HasForeignKey("OrganizationTypeId");
                });

            modelBuilder.Entity("AIMS.Models.EFProject", b =>
                {
                    b.HasOne("AIMS.Models.EFProjectTypes", "ProjectType")
                        .WithMany()
                        .HasForeignKey("ProjectTypeId");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectCustomFields", b =>
                {
                    b.HasOne("AIMS.Models.EFCustomFields", "CustomField")
                        .WithMany("ProjectFieldsList")
                        .HasForeignKey("CustomFieldId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDisbursements", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Disbursements")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDocuments", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Documents")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectFunders", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganization", "Funder")
                        .WithMany()
                        .HasForeignKey("FunderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Funders")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectFundings", b =>
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

            modelBuilder.Entity("AIMS.Models.EFProjectImplementors", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganization", "Implementor")
                        .WithMany()
                        .HasForeignKey("ImplementorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Implementors")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectLocations", b =>
                {
                    b.HasOne("AIMS.Models.EFLocation", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectMarkers", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectSectors", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFSector", "Sector")
                        .WithMany()
                        .HasForeignKey("SectorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFReportSubscriptions", b =>
                {
                    b.HasOne("AIMS.Models.EFStaticReports", "Report")
                        .WithMany()
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFSector", b =>
                {
                    b.HasOne("AIMS.Models.EFSectorCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("AIMS.Models.EFSectorSubCategory", "SubCategory")
                        .WithMany()
                        .HasForeignKey("SubCategoryId");
                });

            modelBuilder.Entity("AIMS.Models.EFSectorCategory", b =>
                {
                    b.HasOne("AIMS.Models.EFSectorTypes", "SectorType")
                        .WithMany("SectorCategories")
                        .HasForeignKey("SectorTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFSectorSubCategory", b =>
                {
                    b.HasOne("AIMS.Models.EFSectorCategory", "SectorCategory")
                        .WithMany()
                        .HasForeignKey("SectorCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFUser", b =>
                {
                    b.HasOne("AIMS.Models.EFUser", "ApprovedBy")
                        .WithMany()
                        .HasForeignKey("ApprovedById");

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
#pragma warning restore 612, 618
        }
    }
}
