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
    [Migration("20191213064816_Added_Label_FYs")]
    partial class Added_Label_FYs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AIMS.Models.EFCurrency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Currency");

                    b.Property<string>("CurrencyName");

                    b.Property<bool>("IsDefault");

                    b.Property<bool>("IsNational");

                    b.Property<int?>("Source");

                    b.HasKey("Id");

                    b.HasIndex("Currency")
                        .IsUnique()
                        .HasFilter("[Currency] IS NOT NULL");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("AIMS.Models.EFDropboxSettings", b =>
                {
                    b.Property<string>("Token")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Token");

                    b.ToTable("DropboxSettings");
                });

            modelBuilder.Entity("AIMS.Models.EFEmailMessages", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FooterMessage");

                    b.Property<string>("Message")
                        .HasMaxLength(1000);

                    b.Property<int>("MessageType");

                    b.Property<string>("Subject")
                        .HasMaxLength(200);

                    b.Property<string>("TypeDefinition")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("EmailMessages");
                });

            modelBuilder.Entity("AIMS.Models.EFEnvelope", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Currency");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(9, 2)");

                    b.Property<int>("FunderId");

                    b.HasKey("Id");

                    b.HasIndex("FunderId");

                    b.ToTable("Envelope");
                });

            modelBuilder.Entity("AIMS.Models.EFEnvelopeTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("EnvelopeTypes");
                });

            modelBuilder.Entity("AIMS.Models.EFEnvelopeYearlyBreakup", b =>
                {
                    b.Property<int>("EnvelopeTypeId");

                    b.Property<int>("EnvelopeId");

                    b.Property<int>("YearId");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("EnvelopeTypeId", "EnvelopeId", "YearId");

                    b.HasIndex("EnvelopeId");

                    b.HasIndex("YearId");

                    b.ToTable("EnvelopeYearlyBreakups");
                });

            modelBuilder.Entity("AIMS.Models.EFExchangeRates", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Dated");

                    b.Property<string>("ExchangeRatesJson");

                    b.HasKey("Id");

                    b.ToTable("ExchangeRates");
                });

            modelBuilder.Entity("AIMS.Models.EFExchangeRatesAPIsCount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Count");

                    b.Property<DateTime>("Dated");

                    b.HasKey("Id");

                    b.ToTable("ExchangeRatesAPIsCount");
                });

            modelBuilder.Entity("AIMS.Models.EFExchangeRatesSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("APIKeyOpenExchangeRates");

                    b.Property<int>("FinancialYearEndingMonth");

                    b.Property<int>("FinancialYearStartingMonth");

                    b.Property<string>("ManualExchangeRateSource");

                    b.Property<string>("ManualExchangeRates");

                    b.HasKey("Id");

                    b.ToTable("ExchangeRatesSettings");
                });

            modelBuilder.Entity("AIMS.Models.EFExchangeRatesUsageSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Order");

                    b.Property<int>("Source");

                    b.Property<int>("UsageSection");

                    b.HasKey("Id");

                    b.HasIndex("Source", "UsageSection")
                        .IsUnique();

                    b.ToTable("ExchangeRatesUsageSettings");
                });

            modelBuilder.Entity("AIMS.Models.EFFinancialYears", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FinancialYear");

                    b.Property<string>("Label");

                    b.HasKey("Id");

                    b.HasIndex("FinancialYear")
                        .IsUnique();

                    b.ToTable("FinancialYears");
                });

            modelBuilder.Entity("AIMS.Models.EFFundingTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FundingType");

                    b.HasKey("Id");

                    b.ToTable("FundingTypes");
                });

            modelBuilder.Entity("AIMS.Models.EFHelp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Entity");

                    b.Property<string>("HelpInfoJson");

                    b.HasKey("Id");

                    b.ToTable("Help");
                });

            modelBuilder.Entity("AIMS.Models.EFHomePageSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AIMSTitle");

                    b.Property<string>("IntroductionHeading");

                    b.Property<string>("IntroductionText");

                    b.HasKey("Id");

                    b.ToTable("HomePageSettings");
                });

            modelBuilder.Entity("AIMS.Models.EFIATIData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Data");

                    b.Property<DateTime>("Dated");

                    b.Property<string>("Organizations");

                    b.HasKey("Id");

                    b.ToTable("IATIData");
                });

            modelBuilder.Entity("AIMS.Models.EFIATIOrganization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("OrganizationName");

                    b.HasKey("Id");

                    b.ToTable("IATIOrganizations");
                });

            modelBuilder.Entity("AIMS.Models.EFIATISettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BaseUrl");

                    b.Property<string>("TransactionTypesJson");

                    b.HasKey("Id");

                    b.ToTable("IATISettings");
                });

            modelBuilder.Entity("AIMS.Models.EFLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal?>("Latitude")
                        .HasColumnType("decimal(9, 5)");

                    b.Property<string>("Location");

                    b.Property<decimal?>("Longitude")
                        .HasColumnType("decimal(9, 5)");

                    b.HasKey("Id");

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

            modelBuilder.Entity("AIMS.Models.EFManualExchangeRates", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Currency");

                    b.Property<string>("DefaultCurrency");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(9, 2)");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("ManualExchangeRates");
                });

            modelBuilder.Entity("AIMS.Models.EFMarkers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FieldTitle");

                    b.Property<int>("FieldType");

                    b.Property<string>("Help");

                    b.Property<string>("Values");

                    b.HasKey("Id");

                    b.ToTable("Markers");
                });

            modelBuilder.Entity("AIMS.Models.EFOrganization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsApproved");

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

            modelBuilder.Entity("AIMS.Models.EFPasswordRecoveryRequests", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Dated");

                    b.Property<string>("Email");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.ToTable("PasswordRecoveryRequests");
                });

            modelBuilder.Entity("AIMS.Models.EFProject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CreatedById");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("EndDate");

                    b.Property<int?>("EndingFinancialYearId");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(9, 2)");

                    b.Property<int>("FundingTypeId");

                    b.Property<string>("ProjectCurrency");

                    b.Property<decimal>("ProjectValue")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int?>("StartingFinancialYearId");

                    b.Property<string>("Title")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("EndingFinancialYearId");

                    b.HasIndex("FundingTypeId");

                    b.HasIndex("StartingFinancialYearId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDeletionRequests", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("UserId");

                    b.Property<DateTime>("RequestedOn");

                    b.Property<int>("Status");

                    b.Property<DateTime>("StatusUpdatedOn");

                    b.HasKey("ProjectId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ProjectDeletionRequests");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDisbursements", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("Currency");

                    b.Property<decimal>("DisbursementType")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(9, 2)");

                    b.Property<int>("ProjectId");

                    b.Property<int?>("YearId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("YearId");

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

                    b.HasKey("ProjectId", "FunderId");

                    b.HasIndex("FunderId");

                    b.ToTable("ProjectFunders");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectImplementers", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("ImplementerId");

                    b.HasKey("ProjectId", "ImplementerId");

                    b.HasIndex("ImplementerId");

                    b.ToTable("ProjectImplementers");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectLocations", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("LocationId");

                    b.Property<decimal>("FundsPercentage")
                        .HasColumnType("decimal(9, 2)");

                    b.HasKey("ProjectId", "LocationId");

                    b.HasIndex("LocationId");

                    b.ToTable("ProjectLocations");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectMarkers", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("MarkerId");

                    b.Property<int>("FieldType");

                    b.Property<string>("Values");

                    b.HasKey("ProjectId", "MarkerId");

                    b.HasIndex("MarkerId");

                    b.ToTable("ProjectMarkers");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectMembershipRequests", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("UserId");

                    b.Property<DateTime>("Dated");

                    b.Property<bool>("IsApproved");

                    b.Property<int>("OrganizationId");

                    b.HasKey("ProjectId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ProjectMembershipRequests");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectSectors", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("SectorId");

                    b.Property<decimal>("FundsPercentage")
                        .HasColumnType("decimal(9, 2)");

                    b.HasKey("ProjectId", "SectorId");

                    b.HasIndex("SectorId");

                    b.ToTable("ProjectSectors");
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

                    b.Property<int?>("ParentSectorId");

                    b.Property<string>("SectorName");

                    b.Property<int>("SectorTypeId");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("Id");

                    b.HasIndex("ParentSectorId");

                    b.HasIndex("SectorTypeId");

                    b.ToTable("Sectors");
                });

            modelBuilder.Entity("AIMS.Models.EFSectorMappings", b =>
                {
                    b.Property<int>("SectorId");

                    b.Property<int>("MappedSectorId");

                    b.Property<int>("SectorTypeId");

                    b.HasKey("SectorId", "MappedSectorId");

                    b.ToTable("SectorMappings");
                });

            modelBuilder.Entity("AIMS.Models.EFSectorTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("IATICode");

                    b.Property<bool?>("IsPrimary");

                    b.Property<bool?>("IsSourceType");

                    b.Property<string>("TypeName");

                    b.HasKey("Id");

                    b.ToTable("SectorTypes");
                });

            modelBuilder.Entity("AIMS.Models.EFSMTPSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdminEmail");

                    b.Property<string>("Host");

                    b.Property<string>("Password");

                    b.Property<int>("Port");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("SMTPSettings");
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

                    b.Property<DateTime?>("ApprovedOn");

                    b.Property<string>("Email");

                    b.Property<bool>("IsApproved");

                    b.Property<DateTime?>("LastLogin");

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

                    b.Property<string>("Email");

                    b.Property<bool>("IsSeen");

                    b.Property<string>("Message");

                    b.Property<int>("NotificationType");

                    b.Property<int?>("OrganizationId");

                    b.Property<int>("TreatmentId");

                    b.Property<int>("UserType");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("UserNotifications");
                });

            modelBuilder.Entity("AIMS.Models.EFEnvelope", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganization", "Funder")
                        .WithMany()
                        .HasForeignKey("FunderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFEnvelopeYearlyBreakup", b =>
                {
                    b.HasOne("AIMS.Models.EFEnvelope", "Envelope")
                        .WithMany()
                        .HasForeignKey("EnvelopeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFEnvelopeTypes", "EnvelopeType")
                        .WithMany()
                        .HasForeignKey("EnvelopeTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFFinancialYears", "Year")
                        .WithMany()
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.Cascade);
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
                    b.HasOne("AIMS.Models.EFUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("AIMS.Models.EFFinancialYears", "EndingFinancialYear")
                        .WithMany()
                        .HasForeignKey("EndingFinancialYearId");

                    b.HasOne("AIMS.Models.EFFundingTypes", "FundingType")
                        .WithMany()
                        .HasForeignKey("FundingTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFFinancialYears", "StartingFinancialYear")
                        .WithMany()
                        .HasForeignKey("StartingFinancialYearId");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDeletionRequests", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFUser", "RequestedBy")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDisbursements", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Disbursements")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFFinancialYears", "Year")
                        .WithMany()
                        .HasForeignKey("YearId");
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

            modelBuilder.Entity("AIMS.Models.EFProjectImplementers", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganization", "Implementer")
                        .WithMany()
                        .HasForeignKey("ImplementerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Implementers")
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
                        .WithMany("Locations")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectMarkers", b =>
                {
                    b.HasOne("AIMS.Models.EFMarkers", "Marker")
                        .WithMany()
                        .HasForeignKey("MarkerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Markers")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectMembershipRequests", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectSectors", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Sectors")
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
                    b.HasOne("AIMS.Models.EFSector", "ParentSector")
                        .WithMany()
                        .HasForeignKey("ParentSectorId");

                    b.HasOne("AIMS.Models.EFSectorTypes", "SectorType")
                        .WithMany("Sectors")
                        .HasForeignKey("SectorTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
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
                        .HasForeignKey("OrganizationId");
                });
#pragma warning restore 612, 618
        }
    }
}
