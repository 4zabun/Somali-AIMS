﻿using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AIMS.Services.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AIMS.Services
{
    public interface IReportService
    {
        /// <summary>
        /// Get projects report by sectors and title
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //Task<ProjectProfileReportBySector> GetProjectsBySector(ReportModelForProjectSectors model);

        /// <summary>
        /// Search matching projects by sector wise grouped for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ProjectProfileReportBySector> GetProjectsBySectors(SearchProjectsBySectorModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Search matching projects without sector for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        Task<ProjectProfileReportBySector> GetProjectsWithoutSectors(SearchProjectsBySectorModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Search matching projects by sector wise grouped for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ProjectProfileReportByLocation> GetProjectsByLocations(SearchProjectsByLocationModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Search project by year in a time series manner
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        Task<TimeSeriesReportByYear> GetProjectsByYear(SearchProjectsByYearModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Gets lighter version of projects budget report
        /// </summary>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        Task<ProjectsBudgetReportSummary> GetProjectsBudgetReportSummary(string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Gets envelope report
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reportUrl"></param>
        /// <param name="defaultCurrency"></param>
        /// <returns></returns>
        Task<EnvelopeReport> GetEnvelopeReport(SearchEnvelopeModel model, string reportUrl, string defaultCurrency, decimal exchangeRate);

        /// <summary>
        /// Gets all projects report
        /// </summary>
        /// <returns></returns>
        Task<ProjectReportView> GetAllProjectsReport(SearchAllProjectsModel model);

        /// <summary>
        /// Gets project report for a single project
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ProjectDetailReport> GetProjectReport(int id, string reportUrl);

        /// <summary>
        /// Internal function for extracting rate of default currency
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="ratesList"></param>
        /// <returns></returns>
        decimal GetExchangeRateForCurrency(string currency, List<CurrencyWithRates> ratesList);

        /// <summary>
        /// Gets projects summary against the location
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectSummary> GetLatestProjectsSummary();

        /// <summary>
        /// Gets location names only for projects report
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetLocationNames();
    }

    public class ReportService : IReportService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ReportService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectSummary> GetLatestProjectsSummary()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ProjectSummary> summaryList = new List<ProjectSummary>();
                var projects = unitWork.ProjectRepository.GetWithInclude(p => p.EndingFinancialYear.FinancialYear >= DateTime.Now.Year, new string[] { "EndingFinancialYear", "Disbursements" })
                    .OrderByDescending(p => p.DateUpdated).Take(10);

                foreach (var project in projects)
                {
                    var disbursements = (from p in project.Disbursements
                                         select (p.Amount * p.ExchangeRate)).Sum();
                    summaryList.Add(new ProjectSummary()
                    {
                        Project = project.Title,
                        Funding = project.ProjectValue,
                        Disbursement = disbursements
                    });
                }
                return summaryList;
            }
        }

        public ICollection<string> GetLocationNames()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var locations = unitWork.LocationRepository.GetProjection(l => l.Id != 0, l => l.Location);
                if (locations.Any())
                {
                    locations = (from l in locations
                                 orderby l ascending
                                 select l);
                }
                return locations.ToList();
            }
        }

        public async Task<ProjectReportView> GetAllProjectsReport(SearchAllProjectsModel model)
        {
            var unitWork = new UnitOfWork(context);
            ProjectReportView projectsReport = new ProjectReportView();
            List<ProjectDetailView> projectsList = new List<ProjectDetailView>();
            List<ProjectDetailSectorView> sectorsList = new List<ProjectDetailSectorView>();
            IQueryable<EFProject> projects;
            int startingFinancialYear = 0;
            int endingFinancialYear = 0;

            var financialYears = unitWork.FinancialYearRepository.GetProjection(y => y.FinancialYear > 0, y => y.FinancialYear);
            var sectors = unitWork.SectorRepository.GetWithInclude(s => s.ParentSectorId != null && s.SectorType.IsPrimary == true, new string[] { "ParentSector" });
            if (sectors.Any())
            {
                var sectorDetailList = (from sec in sectors
                                        orderby sec.SectorName
                          select new { Id = sec.Id, SectorName = sec.SectorName, ParentSector = sec.ParentSector.SectorName });
                foreach(var sec in sectorDetailList)
                {
                    sectorsList.Add(new ProjectDetailSectorView() { Id = sec.Id, Sector = sec.SectorName, ParentSector = sec.ParentSector });
                }
            }
            
            var locations = unitWork.LocationRepository.GetProjection(l => l.Id != 0, l => new { l.Id, l.Location });
            var markers = unitWork.MarkerRepository.GetProjection(m => m.Id != 0, m => new { m.Id, m.FieldTitle });
            projects = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id != 0, new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents", "Markers", "Markers.Marker" });
            startingFinancialYear = financialYears.Min();
            endingFinancialYear = financialYears.Max();

            List<ProjectDetailLocationView> locationsList = new List<ProjectDetailLocationView>();
            if (locations.Any())
            {
                locations = (from l in locations
                             orderby l.Location
                             select l);
                foreach(var location in locations)
                {
                    locationsList.Add(new ProjectDetailLocationView()
                    {
                        Id = location.Id,
                        Location = location.Location
                    });
                }
            }

            List<ProjectDetailMarkerView> markersList = new List<ProjectDetailMarkerView>();
            if (markers.Any())
            {
                markers = (from m in markers
                           orderby m.FieldTitle
                           select m);
                foreach(var marker in markers)
                {
                    markersList.Add(new ProjectDetailMarkerView()
                    {
                        Id = marker.Id,
                        Marker = marker.FieldTitle
                    });
                }
            }

            if (model.StartingYear > 0)
            {
                startingFinancialYear = model.StartingYear;
                projects = (from p in projects
                            where p.StartingFinancialYear.FinancialYear >= model.StartingYear ||
                            p.EndingFinancialYear.FinancialYear >= model.StartingYear
                            select p);
            }

            if (model.EndingYear > 0)
            {
                endingFinancialYear = model.EndingYear;
                projects = (from p in projects
                            where p.EndingFinancialYear.FinancialYear <= model.EndingYear ||
                            p.StartingFinancialYear.FinancialYear <= model.EndingYear
                            select p);
            }

            foreach (var project in projects)
            {
                IEnumerable<string> funderNames = (from f in project.Funders
                                                   select f.Funder.OrganizationName);
                IEnumerable<string> implementerNames = (from i in project.Implementers
                                                        select i.Implementer.OrganizationName);
                IEnumerable<string> organizations = funderNames.Union(implementerNames);

                List<OrganizationAbstractView> fundersList = new List<OrganizationAbstractView>();
                foreach (string org in funderNames)
                {
                    fundersList.Add(new OrganizationAbstractView()
                    {
                        Name = org
                    });
                }

                List<OrganizationAbstractView> implementersList = new List<OrganizationAbstractView>();
                foreach (string org in implementerNames)
                {
                    implementersList.Add(new OrganizationAbstractView()
                    {
                        Name = org
                    });
                }
                projectsList.Add(new ProjectDetailView()
                {
                    Id = project.Id,
                    Title = project.Title.Replace("\"", ""),
                    Description = project.Description,
                    ProjectCurrency = project.ProjectCurrency,
                    ProjectValue = project.ProjectValue,
                    ExchangeRate = project.ExchangeRate,
                    StartingFinancialYear = project.StartingFinancialYear.FinancialYear,
                    EndingFinancialYear = project.EndingFinancialYear.FinancialYear,
                    Funders = fundersList,
                    Implementers = implementersList,
                    Locations = mapper.Map<List<LocationAbstractView>>(project.Locations),
                    Sectors = mapper.Map<List<SectorAbstractView>>(project.Sectors),
                    Documents = mapper.Map<List<DocumentAbstractView>>(project.Documents),
                    Disbursements = mapper.Map<List<DisbursementAbstractView>>(project.Disbursements),
                    Markers = mapper.Map<List<MarkerAbstractView>>(project.Markers)
                });
            }
            projectsReport.StartingFinancialYear = startingFinancialYear;
            projectsReport.EndingFinancialYear = endingFinancialYear;
            projectsReport.Markers = markersList;
            projectsReport.Locations = locationsList;
            projectsReport.Sectors = sectorsList;
            projectsReport.Projects = projectsList;
            return await Task<ProjectReportView>.Run(() => projectsReport).ConfigureAwait(false);
        }

        public async Task<ProjectDetailReport> GetProjectReport(int id, string reportUrl)
        {
            var unitWork = new UnitOfWork(context);
            ProjectDetailReport report = new ProjectDetailReport();
            report.ReportSettings = new Report()
            {
                Title = ReportConstants.PROJECT_PROFILE_TITLE,
                SubTitle = ReportConstants.PROJECT_PROFILE_SUBTITLE,
                Footer = ReportConstants.PROJECTS_PROFILE_FOOTER,
                Dated = DateTime.Now.ToLongDateString(),
                ReportUrl = reportUrl + ReportConstants.PROJECT_PROFILE_URL + id
            };
            ProjectReportView projectsReport = new ProjectReportView();
            List<ProjectDetailView> projectsList = new List<ProjectDetailView>();
            List<ProjectDetailSectorView> sectorsList = new List<ProjectDetailSectorView>();
            IQueryable<EFProject> projects;
            int startingFinancialYear = 0;
            int endingFinancialYear = 0;

            var financialYears = unitWork.FinancialYearRepository.GetProjection(y => y.FinancialYear > 0, y => y.FinancialYear);
            var sectors = unitWork.SectorRepository.GetWithInclude(s => s.ParentSectorId != null && s.SectorType.IsPrimary == true, new string[] { "ParentSector" });
            if (sectors.Any())
            {
                var sectorDetailList = (from sec in sectors
                                        orderby sec.SectorName
                                        select new { Id = sec.Id, SectorName = sec.SectorName, ParentSector = sec.ParentSector.SectorName });
                foreach (var sec in sectorDetailList)
                {
                    sectorsList.Add(new ProjectDetailSectorView() { Id = sec.Id, Sector = sec.SectorName, ParentSector = sec.ParentSector });
                }
            }

            var locations = unitWork.LocationRepository.GetProjection(l => l.Id != 0, l => new { l.Id, l.Location });
            var markers = unitWork.MarkerRepository.GetProjection(m => m.Id != 0, m => new { m.Id, m.FieldTitle });
            projects = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Id == id, new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Disbursements.Year", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents", "Markers", "Markers.Marker" });
            startingFinancialYear = financialYears.Min();
            endingFinancialYear = financialYears.Max();

            List<ProjectDetailLocationView> locationsList = new List<ProjectDetailLocationView>();
            if (locations.Any())
            {
                locations = (from l in locations
                             orderby l.Location
                             select l);
                foreach (var location in locations)
                {
                    locationsList.Add(new ProjectDetailLocationView()
                    {
                        Id = location.Id,
                        Location = location.Location
                    });
                }
            }

            List<ProjectDetailMarkerView> markersList = new List<ProjectDetailMarkerView>();
            if (markers.Any())
            {
                markers = (from m in markers
                           orderby m.FieldTitle
                           select m);
                foreach (var marker in markers)
                {
                    markersList.Add(new ProjectDetailMarkerView()
                    {
                        Id = marker.Id,
                        Marker = marker.FieldTitle
                    });
                }
            }

            var firstProject = (from p in projects
                           select p).FirstOrDefault();
            if (firstProject != null)
            {
                startingFinancialYear = firstProject.StartingFinancialYear.FinancialYear;
                endingFinancialYear = firstProject.EndingFinancialYear.FinancialYear;
            }

            foreach (var project in projects)
            {
                IEnumerable<string> funderNames = (from f in project.Funders
                                                   select f.Funder.OrganizationName);
                IEnumerable<string> implementerNames = (from i in project.Implementers
                                                        select i.Implementer.OrganizationName);
                IEnumerable<string> organizations = funderNames.Union(implementerNames);

                List<OrganizationAbstractView> fundersList = new List<OrganizationAbstractView>();
                foreach (string org in funderNames)
                {
                    fundersList.Add(new OrganizationAbstractView()
                    {
                        Name = org
                    });
                }

                List<OrganizationAbstractView> implementersList = new List<OrganizationAbstractView>();
                foreach (string org in implementerNames)
                {
                    implementersList.Add(new OrganizationAbstractView()
                    {
                        Name = org
                    });
                }
                projectsList.Add(new ProjectDetailView()
                {
                    Id = project.Id,
                    Title = project.Title.Replace("\"", ""),
                    Description = project.Description,
                    ProjectCurrency = project.ProjectCurrency,
                    ProjectValue = project.ProjectValue,
                    ExchangeRate = project.ExchangeRate,
                    StartingFinancialYear = project.StartingFinancialYear.FinancialYear,
                    EndingFinancialYear = project.EndingFinancialYear.FinancialYear,
                    Funders = fundersList,
                    Implementers = implementersList,
                    Locations = mapper.Map<List<LocationAbstractView>>(project.Locations),
                    Sectors = mapper.Map<List<SectorAbstractView>>(project.Sectors),
                    Documents = mapper.Map<List<DocumentAbstractView>>(project.Documents),
                    Disbursements = mapper.Map<List<DisbursementAbstractView>>(project.Disbursements),
                    Markers = mapper.Map<List<MarkerAbstractView>>(project.Markers)
                });
            }
            projectsReport.StartingFinancialYear = startingFinancialYear;
            projectsReport.EndingFinancialYear = endingFinancialYear;
            projectsReport.Markers = markersList;
            projectsReport.Locations = locationsList;
            projectsReport.Sectors = sectorsList;
            projectsReport.Projects = projectsList;
            report.ProjectProfile = projectsReport;
            return await Task<ProjectDetailReport>.Run(() => report).ConfigureAwait(false);
        }

        public async Task<ProjectProfileReportByLocation> GetProjectsByLocations(SearchProjectsByLocationModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                exchangeRate = (exchangeRate == 0) ? 1 : exchangeRate;
                ProjectProfileReportByLocation locationProjectsReport = new ProjectProfileReportByLocation();
                try
                {
                    IQueryStringGenerator queryStringGenerator = new QueryStringGenerator();
                    string queryString = queryStringGenerator.GetQueryStringForLocationsReport(model);
                    reportUrl += ReportConstants.LOCATION_REPORT_URL;

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        reportUrl += queryString;
                    }
                    locationProjectsReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BY_LOCATION_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BY_LOCATION_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BY_LOCATION_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl
                    };

                    DateTime dated = new DateTime();
                    int year = dated.Year;
                    int month = dated.Month;
                    IQueryable<EFProject> projectProfileList = null;
                    IQueryable<EFProjectLocations> projectLocationsQueryable = null;
                    List<EFProjectLocations> projectLocations = null;

                    if (model.ProjectIds.Count > 0)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.StartingYear >= 1970 && model.EndingYear >= 1970)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.EndingFinancialYear.FinancialYear <= model.EndingYear)),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where project.StartingFinancialYear.FinancialYear >= model.StartingYear
                                                 && project.EndingFinancialYear.FinancialYear <= model.EndingYear
                                                 select project;
                        }
                    }

                    if (model.OrganizationIds.Count > 0)
                    {
                        var projectFunders = unitWork.ProjectFundersRepository.GetMany(f => model.OrganizationIds.Contains(f.FunderId));
                        var projectIdsFunders = (from pFunder in projectFunders
                                                 select pFunder.ProjectId).ToList<int>().Distinct();

                        var projectImplementers = unitWork.ProjectImplementersRepository.GetMany(f => model.OrganizationIds.Contains(f.ImplementerId));
                        var projectIdsImplementers = (from pImplementer in projectImplementers
                                                      select pImplementer.ProjectId).ToList<int>().Distinct();


                        var projectIdsList = projectIdsFunders.Union(projectIdsImplementers);

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (model.LocationIds.Count > 0)
                    {
                        projectLocationsQueryable = unitWork.ProjectLocationsRepository.GetWithInclude(l => model.LocationIds.Contains(l.LocationId), new string[] { "Location" });
                    }
                    else
                    {
                        projectLocationsQueryable = unitWork.ProjectLocationsRepository.GetWithInclude(p => p.ProjectId != 0, new string[] { "Location" });
                    }

                    projectLocations = (from pLocation in projectLocationsQueryable
                                        orderby pLocation.Location.Location
                                        select pLocation).ToList();

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.StartingFinancialYear.FinancialYear >= year),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    foreach (var project in projectProfileList)
                    {
                        decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.ProjectValue = project.ProjectValue;
                        profileView.ProjectCurrency = project.ProjectCurrency;
                        profileView.ExchangeRate = projectExchangeRate;
                        profileView.Description = project.Description;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                        profileView.Locations = mapper.Map<List<ProjectLocationDetailView>>(project.Locations);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        projectsList.Add(profileView);
                    }

                    List<ProjectsByLocation> locationProjectsList = new List<ProjectsByLocation>();
                    ProjectsByLocation projectsByLocation = null;

                    int totalLocations = projectLocations.Count();
                    List<ProjectViewForLocation> projectsListForLocation = null;
                    ICollection<LocationProjects> locationsByProjects = new List<LocationProjects>();

                    foreach (var loc in projectLocations)
                    {
                        var isLocationIdsExist = (from locIds in locationsByProjects
                                                  where locIds.LocationId.Equals(loc.LocationId)
                                                  select locIds).FirstOrDefault();

                        if (isLocationIdsExist == null)
                        {
                            locationsByProjects.Add(new LocationProjects()
                            {
                                LocationId = loc.LocationId,
                                Location = loc.Location.Location,
                                Projects = (from locProject in projectLocations
                                            where locProject.LocationId.Equals(loc.LocationId)
                                            select new LocationProject
                                            {
                                                ProjectId = locProject.ProjectId,
                                                FundsPercentage = locProject.FundsPercentage
                                            }).ToList<LocationProject>()
                            });
                        }
                    }

                    locationsByProjects = (from loc in locationsByProjects
                                           orderby loc.Location
                                           select loc).ToList();
                    foreach (var locationByProject in locationsByProjects)
                    {
                        projectsByLocation = new ProjectsByLocation();
                        projectsByLocation.LocationName = locationByProject.Location;
                        int currentLocationId = locationByProject.LocationId;

                        projectsListForLocation = new List<ProjectViewForLocation>();
                        var projectIds = (from p in locationByProject.Projects
                                          select p.ProjectId);

                        var locationProjects = (from project in projectsList
                                                where projectIds.Contains(project.Id)
                                                select project).ToList<ProjectProfileView>();

                        int projectTotalPercentage = 0;
                        decimal totalFundingPercentage = 0, totalDisbursements = 0, totalDisbursementsPercentage = 0, locationPercentage = 0,
                            locationActualDisbursements = 0, locationPlannedDisbursements = 0;
                        foreach (var project in locationProjects)
                        {
                            if (project.Locations != null)
                            {
                                locationPercentage = (from l in project.Locations
                                                      where l.LocationId == currentLocationId
                                                      select l.FundsPercentage).FirstOrDefault();

                                projectTotalPercentage += Convert.ToInt32(locationPercentage);
                                project.ProjectPercentValue = Math.Round(((project.ProjectValue / 100) * locationPercentage), MidpointRounding.AwayFromZero);
                                totalFundingPercentage += project.ProjectPercentValue;
                            }
                        }

                        foreach (var project in locationProjects)
                        {
                            if (project.Locations != null)
                            {
                                locationPercentage = (from l in project.Locations
                                                      where l.LocationId == currentLocationId
                                                      select l.FundsPercentage).FirstOrDefault();
                                if (project.Disbursements.Count() > 0)
                                {
                                    if (model.StartingYear >= 2000)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year >= model.StartingYear
                                                                 select d).ToList();
                                    }

                                    if (model.EndingYear >= 2000)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year <= model.EndingYear
                                                                 select d).ToList();
                                    }


                                    decimal actualDisbursements = ((((from d in project.Disbursements
                                                                      where d.DisbursementType == DisbursementTypes.Actual
                                                                      select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum()) / 100) * locationPercentage);
                                    decimal plannedDisbursements = ((((from d in project.Disbursements
                                                                       where d.DisbursementType == DisbursementTypes.Planned
                                                                       select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum()) / 100) * locationPercentage);

                                    totalDisbursements += actualDisbursements;
                                    UtilityHelper helper = new UtilityHelper();
                                    project.ActualDisbursements = Math.Round(actualDisbursements, MidpointRounding.AwayFromZero);
                                    project.PlannedDisbursements = Math.Round(plannedDisbursements, MidpointRounding.AwayFromZero);
                                    totalDisbursementsPercentage += project.ActualDisbursements;
                                    locationActualDisbursements += project.ActualDisbursements;
                                    locationPlannedDisbursements += project.PlannedDisbursements;
                                }
                            }
                        }

                        foreach (var project in locationProjects)
                        {
                            projectsListForLocation.Add(new ProjectViewForLocation()
                            {
                                Title = project.Title.Replace("\"", ""),
                                StartingFinancialYear = project.StartingFinancialYear,
                                EndingFinancialYear = project.EndingFinancialYear,
                                Funders = string.Join(",", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectValue = (project.ProjectValue * (exchangeRate / project.ExchangeRate)),
                                ProjectPercentValue = project.ProjectPercentValue,
                                ActualDisbursements = project.ActualDisbursements,
                                PlannedDisbursements = project.PlannedDisbursements,
                            });
                        }
                        projectsListForLocation = (from pl in projectsListForLocation
                                                   orderby pl.Title ascending
                                                   select pl).ToList();
                        projectsByLocation.TotalFunding = Math.Round(totalFundingPercentage, MidpointRounding.AwayFromZero);
                        projectsByLocation.TotalDisbursements = Math.Round(totalDisbursementsPercentage, MidpointRounding.AwayFromZero);
                        projectsByLocation.ActualDisbursements = Math.Round(locationActualDisbursements, MidpointRounding.AwayFromZero);
                        projectsByLocation.PlannedDisbursements = Math.Round(locationPlannedDisbursements, MidpointRounding.AwayFromZero);
                        projectsByLocation.Projects = projectsListForLocation;
                        locationProjectsList.Add(projectsByLocation);
                    }
                    locationProjectsReport.LocationProjectsList = locationProjectsList;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                return await Task<ProjectProfileReportByLocation>.Run(() => locationProjectsReport).ConfigureAwait(false);
            }
        }

        public async Task<ProjectsBudgetReportSummary> GetProjectsBudgetReportSummary(string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                if (exchangeRate == 0)
                {
                    exchangeRate = 1;
                }
                ProjectsBudgetReportSummary budgetReport = new ProjectsBudgetReportSummary();
                IQueryable<EFProject> projectProfileList = null;
                try
                {
                    budgetReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BUDGET_REPORT_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BUDGET_REPORT_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BUDGET_REPORT_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl + ReportConstants.BUDGET_REPORT_URL + "?load=true"
                    };

                    int currentYear = DateTime.Now.Year;
                    int previousYear = (currentYear - 1);
                    int futureYearsLimit = (currentYear + 3);
                    List<ProjectBudgetSummaryView> projectBudgetsList = new List<ProjectBudgetSummaryView>();

                    projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.EndingFinancialYear.FinancialYear >= currentYear,
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Disbursements", "Disbursements.Year" });

                    projectProfileList = (from p in projectProfileList
                                          orderby p.DateUpdated descending
                                          select p);

                    UtilityHelper utilityHelper = new UtilityHelper();
                    List<YearlyTotalDisbursementsSummary> totalDisbursementsSummaryList = new List<YearlyTotalDisbursementsSummary>();
                    foreach (var project in projectProfileList)
                    {
                        ProjectBudgetSummaryView projectBudget = new ProjectBudgetSummaryView();
                        int upperYearLimit = currentYear + 3;
                        int yearsLeft = upperYearLimit - currentYear;
                        int projectStartYear = project.StartingFinancialYear.FinancialYear;
                        projectBudget.Id = project.Id;
                        projectBudget.Title = project.Title.Replace("\"", "");

                        List<ProjectDisbursements> disbursementsList = new List<ProjectDisbursements>();
                        decimal projectCost = 0;

                        List<ProjectYearlyDisbursementsSummary> yearlyDisbursements = new List<ProjectYearlyDisbursementsSummary>();
                        List<ProjectYearlyDisbursementsBreakup> disbursementsBreakup = new List<ProjectYearlyDisbursementsBreakup>();
                        decimal actualDisbursements = 0, expectedDisbursements = 0, disbursements = 0;
                        ++upperYearLimit;
                        for (int year = (currentYear - 1); year < upperYearLimit; year++)
                        {
                            decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                            projectCost = (project.ProjectValue * (exchangeRate / projectExchangeRate));
                            yearsLeft = upperYearLimit - year;
                            decimal yearDisbursements = Math.Round((from d in project.Disbursements
                                                                    where d.Year.FinancialYear == year && d.DisbursementType == DisbursementTypes.Actual
                                                                    select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);

                            actualDisbursements += yearDisbursements;
                            expectedDisbursements = Math.Round((from d in project.Disbursements
                                                                where d.Year.FinancialYear == year && d.DisbursementType == DisbursementTypes.Planned
                                                                select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);

                            disbursements = yearDisbursements + expectedDisbursements;
                            actualDisbursements += expectedDisbursements;
                            yearlyDisbursements.Add(new ProjectYearlyDisbursementsSummary()
                            {
                                Year = year,
                                Disbursements = disbursements,
                            });

                            disbursementsBreakup.Add(new ProjectYearlyDisbursementsBreakup()
                            {
                                Year = year,
                                ActualDisbursements = yearDisbursements,
                                ExpectedDisbursements = expectedDisbursements
                            });

                            var yearExists = (from s in totalDisbursementsSummaryList
                                              where s.Year == year
                                              select s).FirstOrDefault();

                            if (yearExists == null)
                            {
                                totalDisbursementsSummaryList.Add(new YearlyTotalDisbursementsSummary()
                                {
                                    Year = year,
                                    TotalDisbursements = yearDisbursements,
                                    TotalExpectedDisbursements = expectedDisbursements
                                });
                            }
                            else
                            {
                                yearExists.TotalDisbursements += yearDisbursements;
                                yearExists.TotalExpectedDisbursements += expectedDisbursements;
                            }
                        }
                        projectBudget.YearlyDisbursements = yearlyDisbursements;
                        projectBudget.DisbursementsBreakup = disbursementsBreakup;
                        projectBudgetsList.Add(projectBudget);
                    }
                    foreach (var total in totalDisbursementsSummaryList)
                    {
                        total.TotalDisbursements = Math.Round(total.TotalDisbursements, MidpointRounding.AwayFromZero);
                        total.TotalExpectedDisbursements = Math.Round(total.TotalExpectedDisbursements, MidpointRounding.AwayFromZero);
                    }
                    budgetReport.TotalYearlyDisbursements = totalDisbursementsSummaryList;
                    projectBudgetsList = (from pl in projectBudgetsList
                                          orderby pl.Title ascending
                                          select pl).ToList();
                    budgetReport.Projects = projectBudgetsList;
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
                return await Task<ProjectsBudgetReportSummary>.Run(() => budgetReport).ConfigureAwait(false);
            }
        }

        public async Task<EnvelopeReport> GetEnvelopeReport(SearchEnvelopeModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                EnvelopeReport envelopeReport = new EnvelopeReport();
                try
                {
                    IQueryStringGenerator queryStringGenerator = new QueryStringGenerator();
                    string queryString = queryStringGenerator.GetQueryStringForEnvelopeReport(model);
                    reportUrl += ReportConstants.ENVELOPE_REPORT_URL;

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        reportUrl += queryString;
                    }
                    envelopeReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_ENVELOPE_REPORT_TITLE,
                        SubTitle = ReportConstants.PROJECTS_ENVELOPE_REPORT_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_ENVELOPE_REPORT_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl
                    };

                    int currentYear = DateTime.Now.Year;
                    int previousYear = currentYear - 1;
                    int upperYearLimit = currentYear + 1;
                    List<EnvelopeYearlyView> envelopeViewList = new List<EnvelopeYearlyView>();
                    IQueryable<EFEnvelopeYearlyBreakup> envelopeList = null;
                    IQueryable<EFEnvelope> envelopes = null;

                    var envelopeTypes = unitWork.EnvelopeTypesRepository.GetManyQueryable(e => e.Id != 0);
                    envelopeReport.EnvelopeTypes = mapper.Map<List<EnvelopeTypeView>>(envelopeTypes);
                    if (model.FunderIds.Count > 0)
                    {
                        envelopes = unitWork.EnvelopeRepository.GetWithInclude(e => model.FunderIds.Contains(e.FunderId), new string[] { "Funder" });
                        envelopeList = unitWork.EnvelopeYearlyBreakupRepository.GetWithInclude(e => model.FunderIds.Contains(e.Envelope.FunderId), new string[] { "Envelope", "Year" });
                    }
                    else if (model.FunderTypeIds.Count > 0)
                    {
                        envelopes = unitWork.EnvelopeRepository.GetWithInclude(e => model.FunderTypeIds.Contains((int)e.Funder.OrganizationTypeId), new string[] { "Funder", "Funder.OrganizationType" });
                        var funderIds = (from e in envelopes
                                         select e.FunderId);
                        envelopeList = unitWork.EnvelopeYearlyBreakupRepository.GetWithInclude(e => funderIds.Contains((int)e.Envelope.FunderId), new string[] { "Envelope", "Year" });
                    }
                    else
                    {
                        envelopes = unitWork.EnvelopeRepository.GetWithInclude(e => e.FunderId != 0, new string[] { "Funder" });
                        envelopeList = unitWork.EnvelopeYearlyBreakupRepository.GetWithInclude(e => e.Envelope.FunderId != 0, new string[] { "Envelope", "Year" });
                    }

                    if (model.StartingYear > 0)
                    {
                        previousYear = model.StartingYear;
                        envelopeList = (from e in envelopeList
                                        where e.Year.FinancialYear >= model.StartingYear
                                        select e);
                    }

                    if (model.EndingYear > 0)
                    {
                        upperYearLimit = model.EndingYear;
                        envelopeList = (from e in envelopeList
                                        where e.Year.FinancialYear <= model.EndingYear
                                        select e);
                    }

                    if (model.EnvelopeTypeIds.Count > 0)
                    {
                        envelopeList = (from e in envelopeList
                                        where model.EnvelopeTypeIds.Contains(e.EnvelopeTypeId)
                                        select e);

                        envelopeTypes = (from e in envelopeTypes
                                         where model.EnvelopeTypeIds.Contains(e.Id)
                                         select e);
                    }

                    List<int> envelopeYearsList = new List<int>();
                    for (int yr = previousYear; yr <= upperYearLimit; yr++)
                    {
                        envelopeYearsList.Add(yr);
                    }
                    envelopeReport.EnvelopeYears = envelopeYearsList;
                    foreach (var envelope in envelopes)
                    {
                        EnvelopeYearlyView envelopeView = new EnvelopeYearlyView();
                        envelopeView.EnvelopeBreakupsByType = new List<EnvelopeBreakupView>();
                        IQueryable<EFEnvelopeYearlyBreakup> yearlyBreakup = null;
                        envelopeView.Currency = envelope.Currency;
                        envelopeView.ExchangeRate = envelope.ExchangeRate;
                        envelopeView.FunderId = envelope.FunderId;
                        envelopeView.Funder = envelope.Funder.OrganizationName;
                        int envelopeId = envelope.Id;

                        yearlyBreakup = (from yb in envelopeList
                                         where yb.EnvelopeId == envelope.Id
                                         orderby yb.Year.FinancialYear ascending
                                         select yb);

                        IQueryable<EFEnvelopeYearlyBreakup> yearBreakup = null;
                        decimal envelopeTypeTotal = 0;
                        foreach (var type in envelopeTypes)
                        {
                            EnvelopeBreakupView breakupView = new EnvelopeBreakupView();
                            breakupView.EnvelopeType = type.TypeName;
                            breakupView.EnvelopeTypeId = type.Id;
                            envelopeTypeTotal = 0;
                            List<EnvelopeYearlyBreakUp> yearlyBreakupList = new List<EnvelopeYearlyBreakUp>();
                            for (int year = previousYear; year <= upperYearLimit; year++)
                            {
                                if (yearlyBreakup != null)
                                {
                                    yearBreakup = (from yb in yearlyBreakup
                                                   where yb.Year.FinancialYear == year
                                                   select yb);
                                }

                                EFEnvelopeYearlyBreakup isBreakupExists = null;
                                if (yearBreakup != null)
                                {
                                    isBreakupExists = (from typeBreakup in yearBreakup
                                                       where typeBreakup.EnvelopeTypeId == type.Id
                                                       select typeBreakup).FirstOrDefault();
                                }

                                if (isBreakupExists != null)
                                {
                                    var amount = Math.Round(isBreakupExists.Amount * (exchangeRate / envelope.ExchangeRate), MidpointRounding.AwayFromZero);
                                    yearlyBreakupList.Add(new EnvelopeYearlyBreakUp()
                                    {
                                        Amount = amount,
                                        Year = year
                                    });
                                    envelopeTypeTotal += amount;
                                }
                                else
                                {
                                    yearlyBreakupList.Add(new EnvelopeYearlyBreakUp()
                                    {
                                        Amount = 0,
                                        Year = year,
                                    });
                                }
                            }

                            if (envelopeTypeTotal != 0)
                            {
                                breakupView.YearlyBreakup = yearlyBreakupList;
                                envelopeView.EnvelopeBreakupsByType.Add(breakupView);
                            }
                        }

                        if (envelopeTypeTotal != 0)
                        {
                            envelopeViewList.Add(envelopeView);
                        }
                    }
                    envelopeViewList = (from e in envelopeViewList
                                        orderby e.Funder ascending
                                        select e).ToList();
                    envelopeReport.Envelope = envelopeViewList;
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
                return await Task<EnvelopeReport>.Run(() => envelopeReport).ConfigureAwait(false);
            }
        }

        public async Task<ProjectProfileReportBySector> GetProjectsWithoutSectors(SearchProjectsBySectorModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                exchangeRate = (exchangeRate == 0) ? 1 : exchangeRate;
                ProjectProfileReportBySector sectorProjectsReport = new ProjectProfileReportBySector();
                try
                {
                    IQueryStringGenerator queryStringGenerator = new QueryStringGenerator();
                    string queryString = queryStringGenerator.GetQueryStringForSectorsReport(model);
                    reportUrl += ReportConstants.SECTOR_REPORT_URL;

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        reportUrl += queryString;
                    }
                    sectorProjectsReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BY_SECTOR_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BY_SECTOR_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BY_SECTOR_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl
                    };

                    DateTime dated = new DateTime();
                    int year = dated.Year;
                    int month = dated.Month;
                    IQueryable<EFProject> projectProfileList = null;
                    List<int> locationProjectIds = new List<int>();

                    var projectIds = unitWork.ProjectSectorsRepository.GetProjection(p => p.ProjectId != 0, p => p.ProjectId).Distinct();
                    if (model.LocationId != 0)
                    {
                        locationProjectIds = unitWork.ProjectLocationsRepository.GetProjection(p => !projectIds.Contains(p.ProjectId) && p.LocationId == model.LocationId, p => p.ProjectId).ToList();
                    }

                    if (locationProjectIds.Count > 0)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => !projectIds.Contains(p.Id) && locationProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.ProjectIds.Count > 0)
                    {
                        model.ProjectIds = model.ProjectIds.Except(projectIds).ToList();
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = (from p in projectProfileList
                                                  where model.ProjectIds.Contains(p.Id)
                                                  select p);
                        }
                    }

                    if (model.StartingYear >= 2000 && model.EndingYear >= 2000)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => !projectIds.Contains(p.Id) && ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.EndingFinancialYear.FinancialYear <= model.EndingYear)),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where !projectIds.Contains(project.Id) && project.StartingFinancialYear.FinancialYear >= model.StartingYear
                                                 && project.EndingFinancialYear.FinancialYear <= model.EndingYear
                                                 select project;
                        }
                    }

                    if (model.OrganizationIds.Count > 0)
                    {
                        var projectFunders = unitWork.ProjectFundersRepository.GetMany(f => model.OrganizationIds.Contains(f.FunderId));
                        var projectIdsFunders = (from pFunder in projectFunders
                                                 select pFunder.ProjectId).ToList<int>().Distinct();

                        var projectImplementers = unitWork.ProjectImplementersRepository.GetMany(f => model.OrganizationIds.Contains(f.ImplementerId));
                        var projectIdsImplementers = (from pImplementer in projectImplementers
                                                      select pImplementer.ProjectId).ToList<int>().Distinct();


                        var projectIdsList = (projectIdsFunders.Union(projectIdsImplementers)).Except(projectIds);
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => !projectIds.Contains(p.Id) && (p.EndingFinancialYear.FinancialYear >= year),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }
                    
                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    foreach (var project in projectProfileList)
                    {
                        decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.ProjectCurrency = project.ProjectCurrency;
                        profileView.ProjectValue = project.ProjectValue;
                        profileView.ExchangeRate = projectExchangeRate;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        projectsList.Add(profileView);
                    }

                        decimal totalFunding = 0, totalDisbursements = 0, actualDisbursements = 0, 
                        plannedDisbursements = 0, totalSectorDisbursements = 0, totalSectorActualDisbursements = 0,
                        totalSectorPlannedDisbursements = 0;

                    List<ProjectsBySector> sectorProjectsList = new List<ProjectsBySector>();
                    List<ProjectViewForSector> projectsListForSector = new List<ProjectViewForSector>();
                    ProjectsBySector noSector = new ProjectsBySector();
                    noSector.ParentSector = null;
                    noSector.ParentSectorId = 0;
                    noSector.SectorName = "Not having a sector";

                    foreach(var project in projectsList)
                    {
                        var projectExchangeRate = project.ExchangeRate;
                        totalFunding += Math.Round((project.ProjectValue * (exchangeRate / projectExchangeRate)), MidpointRounding.AwayFromZero);
                        actualDisbursements = (from d in project.Disbursements
                                               where d.DisbursementType == DisbursementTypes.Actual
                                               let disbursements = (d.Amount * (exchangeRate / d.ExchangeRate))
                                               select disbursements).Sum();
                        plannedDisbursements = (from d in project.Disbursements
                                               where d.DisbursementType == DisbursementTypes.Planned
                                               let disbursements = (d.Amount * (exchangeRate / d.ExchangeRate))
                                               select disbursements).Sum();
                        
                        totalDisbursements = (actualDisbursements + plannedDisbursements);
                        totalSectorActualDisbursements = Math.Round((totalSectorActualDisbursements + actualDisbursements), MidpointRounding.AwayFromZero);
                        totalSectorPlannedDisbursements = Math.Round((totalSectorPlannedDisbursements + plannedDisbursements), MidpointRounding.AwayFromZero);
                        totalSectorDisbursements = Math.Round((totalSectorDisbursements + totalDisbursements), MidpointRounding.AwayFromZero);

                        projectsListForSector.Add(new ProjectViewForSector()
                        {
                            ProjectId = project.Id,
                            Title = project.Title.Replace("\"", ""),
                            StartingFinancialYear = project.StartingFinancialYear,
                            EndingFinancialYear = project.EndingFinancialYear,
                            Funders = string.Join(",", project.Funders.Select(f => f.Funder)),
                            Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                            ProjectValue = Math.Round((project.ProjectValue * (exchangeRate / projectExchangeRate)), MidpointRounding.AwayFromZero),
                            ProjectPercentValue = project.ProjectPercentValue,
                            ActualDisbursements = Math.Round(actualDisbursements, MidpointRounding.AwayFromZero),
                            PlannedDisbursements = Math.Round(plannedDisbursements, MidpointRounding.AwayFromZero),
                        });
                    }
                    noSector.TotalFunding = totalFunding;
                    noSector.TotalDisbursements = totalSectorDisbursements;
                    noSector.ActualDisbursements = totalSectorActualDisbursements;
                    noSector.PlannedDisbursements = totalSectorPlannedDisbursements;
                    noSector.Projects = projectsListForSector;
                    
                    sectorProjectsList.Add(noSector);
                    sectorProjectsReport.SectorProjectsList = sectorProjectsList;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                return await Task<ProjectProfileReportBySector>.Run(() => sectorProjectsReport).ConfigureAwait(false);
            }
        }

        public async Task<ProjectProfileReportBySector> GetProjectsBySectors(SearchProjectsBySectorModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                exchangeRate = (exchangeRate == 0) ? 1 : exchangeRate;
                ProjectProfileReportBySector sectorProjectsReport = new ProjectProfileReportBySector();
                try
                {
                    IQueryStringGenerator queryStringGenerator = new QueryStringGenerator();
                    string queryString = queryStringGenerator.GetQueryStringForSectorsReport(model);
                    reportUrl += ReportConstants.SECTOR_REPORT_URL;

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        reportUrl += queryString;
                    }
                    sectorProjectsReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BY_SECTOR_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BY_SECTOR_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BY_SECTOR_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl
                    };

                    DateTime dated = new DateTime();
                    int year = dated.Year;
                    int month = dated.Month;
                    IQueryable<EFProject> projectProfileList = null;
                    IQueryable<EFProjectSectors> projectSectors = null;
                    List<int> locationProjectIds = new List<int>();

                    if (model.LocationId != 0)
                    {
                        locationProjectIds = unitWork.ProjectLocationsRepository.GetProjection(p => p.LocationId == model.LocationId, p => p.ProjectId).ToList();
                    }

                    if (locationProjectIds.Count > 0)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => locationProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.ProjectIds.Count > 0)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = (from p in projectProfileList
                                                  where model.ProjectIds.Contains(p.Id)
                                                  select p);
                        }
                    }

                    if (model.StartingYear >= 2000 && model.EndingYear >= 2000)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartingFinancialYear.FinancialYear >= model.StartingYear && p.EndingFinancialYear.FinancialYear <= model.EndingYear)),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where project.StartingFinancialYear.FinancialYear >= model.StartingYear
                                                 && project.EndingFinancialYear.FinancialYear <= model.EndingYear
                                                 select project;
                        }
                    }

                    if (model.OrganizationIds.Count > 0)
                    {
                        var projectFunders = unitWork.ProjectFundersRepository.GetMany(f => model.OrganizationIds.Contains(f.FunderId));
                        var projectIdsFunders = (from pFunder in projectFunders
                                                 select pFunder.ProjectId).ToList<int>().Distinct();

                        var projectImplementers = unitWork.ProjectImplementersRepository.GetMany(f => model.OrganizationIds.Contains(f.ImplementerId));
                        var projectIdsImplementers = (from pImplementer in projectImplementers
                                                      select pImplementer.ProjectId).ToList<int>().Distinct();


                        var projectIdsList = projectIdsFunders.Union(projectIdsImplementers);

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }
                    
                    List<int> sectorIds = new List<int>();
                    if (model.SectorIds.Count > 0)
                    {
                        projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => model.SectorIds.Contains(p.SectorId) || model.SectorIds.Contains((int)p.Sector.ParentSectorId), new string[] { "Sector" });
                        List<int> projectIdsList = (from s in projectSectors
                                         select s.ProjectId).ToList<int>();

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }
                    else
                    {
                        int defaultSectorTypeId = 0;
                        var defaultSectorType = unitWork.SectorTypesRepository.GetOne(s => s.IsPrimary == true);
                        if (defaultSectorType != null)
                        {
                            defaultSectorTypeId = defaultSectorType.Id;
                        }
                        var parentSectorIds = unitWork.SectorRepository.GetProjection(s => s.ParentSectorId != null && s.SectorTypeId == defaultSectorTypeId, s => s.Id);
                        projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => parentSectorIds.Contains(p.SectorId), new string[] { "Sector" });
                        var projectIdsList = (from s in projectSectors
                                              select s.ProjectId);

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.EndingFinancialYear.FinancialYear >= year),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    projectSectors = from pSector in projectSectors
                                     orderby pSector.Sector.SectorName
                                     select pSector;

                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    foreach (var project in projectProfileList)
                    {
                        decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.ProjectCurrency = project.ProjectCurrency;
                        profileView.ProjectValue = (project.ProjectValue * (exchangeRate / projectExchangeRate));
                        profileView.ExchangeRate = projectExchangeRate;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        projectsList.Add(profileView);
                    }

                    List<ProjectsBySector> sectorProjectsList = new List<ProjectsBySector>();
                    ProjectsBySector projectsBySector = null;

                    int totalSectors = projectSectors.Count();
                    List<ProjectViewForSector> projectsListForSector = null;
                    ICollection<SectorWithProjects> sectorsByProjects = new List<SectorWithProjects>();

                    var sectors = unitWork.SectorRepository.GetProjection(s => s.Id != 0, s => new { s.Id, s.SectorName });
                    projectSectors = (from ps in projectSectors
                                      orderby ps.Sector.SectorName
                                      select ps);

                    foreach (var sec in projectSectors)
                    {
                        var isSectorIdsExist = (from secIds in sectorsByProjects
                                                where secIds.SectorId.Equals(sec.SectorId)
                                                select secIds).FirstOrDefault();

                        if (isSectorIdsExist == null)
                        {
                            sectorsByProjects.Add(new SectorWithProjects()
                            {
                                SectorId = sec.SectorId,
                                ParentSectorId = (int)sec.Sector.ParentSectorId,
                                Sector = sec.Sector.SectorName,
                                Projects = (from secProject in projectSectors
                                            where secProject.SectorId.Equals(sec.SectorId)
                                            select new SectorProject
                                            {
                                                ProjectId = secProject.ProjectId,
                                                FundsPercentage = secProject.FundsPercentage
                                            }).ToList<SectorProject>()
                            });
                        }
                    }

                    sectorsByProjects = (from sec in sectorsByProjects
                                         orderby sec.Sector
                                         select sec).ToList();
                    foreach (var sectorByProject in sectorsByProjects)
                    {
                        projectsBySector = new ProjectsBySector();
                        projectsBySector.SectorName = sectorByProject.Sector;
                        projectsBySector.ParentSectorId = sectorByProject.ParentSectorId;
                        if (projectsBySector.ParentSectorId > 0)
                        {
                            projectsBySector.ParentSector = (from s in sectors
                                                             where s.Id.Equals(projectsBySector.ParentSectorId)
                                                             select s.SectorName).FirstOrDefault();
                        }
                        int currentSectorId = sectorByProject.SectorId;

                        projectsListForSector = new List<ProjectViewForSector>();
                        var projectIds = (from p in sectorByProject.Projects
                                          select p.ProjectId);

                        var sectorProjects = (from project in projectsList
                                              where projectIds.Contains(project.Id)
                                              select project).ToList<ProjectProfileView>();

                        decimal totalFundingPercentage = 0, totalDisbursements = 0, totalDisbursementsPercentage = 0, sectorPercentage = 0,
                            sectorActualDisbursements = 0, sectorPlannedDisbursements = 0;
                        foreach (var project in sectorProjects)
                        {
                            if (project.Sectors != null)
                            {
                                sectorPercentage = (from s in project.Sectors
                                                    where s.SectorId == currentSectorId
                                                    select s.FundsPercentage).FirstOrDefault();

                                if (project.Funders.Count() > 0)
                                {
                                    project.ProjectPercentValue = Math.Round(((project.ProjectValue / 100) * sectorPercentage), MidpointRounding.AwayFromZero);
                                    totalFundingPercentage += project.ProjectPercentValue;
                                }
                            }
                        }

                        foreach (var project in sectorProjects)
                        {
                            if (project.Sectors != null)
                            {
                                sectorPercentage = (from s in project.Sectors
                                                    where s.SectorId == currentSectorId
                                                    select s.FundsPercentage).FirstOrDefault();
                                if (project.Disbursements.Count() > 0)
                                {
                                    if (model.StartingYear >= 2000)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year >= model.StartingYear
                                                                 select d).ToList();
                                    }

                                    if (model.EndingYear >= 2000)
                                    {
                                        project.Disbursements = (from d in project.Disbursements
                                                                 where d.Year <= model.EndingYear
                                                                 select d).ToList();
                                    }

                                    decimal actualDisbursements = (from d in project.Disbursements
                                                                   where d.DisbursementType == DisbursementTypes.Actual
                                                                   select (d.Amount * ( exchangeRate / d.ExchangeRate))).FirstOrDefault();
                                    decimal plannedDisbursements = (from d in project.Disbursements
                                                                    where d.DisbursementType == DisbursementTypes.Planned
                                                                    select (d.Amount * (exchangeRate / d.ExchangeRate))).FirstOrDefault();
                                    totalDisbursements = (actualDisbursements + plannedDisbursements);

                                    UtilityHelper helper = new UtilityHelper();
                                    project.ActualDisbursements = Math.Round(((actualDisbursements / 100) * sectorPercentage), MidpointRounding.AwayFromZero);
                                    project.PlannedDisbursements = Math.Round(((plannedDisbursements / 100) * sectorPercentage), MidpointRounding.AwayFromZero);
                                    if (project.PlannedDisbursements < 0)
                                    {
                                        project.PlannedDisbursements = 0;
                                    }
                                    totalDisbursementsPercentage += project.ActualDisbursements;
                                    sectorActualDisbursements += project.ActualDisbursements;
                                    sectorPlannedDisbursements += project.PlannedDisbursements;
                                }
                            }
                        }

                        foreach (var project in sectorProjects)
                        {
                            decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                            projectsListForSector.Add(new ProjectViewForSector()
                            {
                                ProjectId = project.Id,
                                Title = project.Title.Replace("\"", ""),
                                StartingFinancialYear = project.StartingFinancialYear,
                                EndingFinancialYear = project.EndingFinancialYear,
                                Funders = string.Join(",", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectValue = (project.ProjectValue * (exchangeRate / projectExchangeRate)),
                                ProjectPercentValue = project.ProjectPercentValue,
                                ActualDisbursements = project.ActualDisbursements,
                                PlannedDisbursements = project.PlannedDisbursements,
                            });
                        }

                        projectsListForSector = (from pl in projectsListForSector
                                                 orderby pl.Title ascending
                                                 select pl).ToList();

                        projectsBySector.TotalFunding = totalFundingPercentage;
                        projectsBySector.TotalDisbursements = totalDisbursementsPercentage;
                        projectsBySector.ActualDisbursements = sectorActualDisbursements;
                        projectsBySector.PlannedDisbursements = sectorPlannedDisbursements;
                        projectsBySector.Projects = projectsListForSector;
                        sectorProjectsList.Add(projectsBySector);
                    }
                    sectorProjectsReport.SectorProjectsList = sectorProjectsList;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                return await Task<ProjectProfileReportBySector>.Run(() => sectorProjectsReport).ConfigureAwait(false);
            }
        }

        public async Task<TimeSeriesReportByYear> GetProjectsByYear(SearchProjectsByYearModel model, string reportUrl, string defaultCurrency, decimal exchangeRate)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                TimeSeriesReportByYear timeSeriesReportByYear = new TimeSeriesReportByYear();
                try
                {
                    IQueryStringGenerator queryStringGenerator = new QueryStringGenerator();
                    string queryString = queryStringGenerator.GetQueryStringForTimeSeriesReport(model);
                    reportUrl += ReportConstants.YEARLY_REPORT_URL;

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        reportUrl += queryString;
                    }
                    timeSeriesReportByYear.ReportSettings = new Report()
                    {
                        Title = ReportConstants.TIME_SERIES_REPORT_TITLE,
                        SubTitle = ReportConstants.TIME_SERIES_REPORT_SUBTITLE,
                        Footer = ReportConstants.TIME_SERIES_REPORT_FOOTER,
                        Dated = DateTime.Now.ToLongDateString(),
                        ReportUrl = reportUrl
                    };

                    DateTime dated = new DateTime();
                    int year = dated.Year;
                    int month = dated.Month;
                    IQueryable<EFProject> projectProfileList = null;
                    IEnumerable<int> financialYears = null;

                    if (model.ProjectIds.Count > 0)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => model.ProjectIds.Contains(p.Id),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.StartingYear >= 2000 && model.EndingYear >= 2000)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.EndingFinancialYear.FinancialYear >= model.StartingYear || p.EndingFinancialYear.FinancialYear <= model.EndingYear)),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where project.StartingFinancialYear.FinancialYear >= model.StartingYear
                                                 && project.EndingFinancialYear.FinancialYear <= model.EndingYear
                                                 select project;
                        }
                    }

                    if (model.OrganizationIds.Count > 0)
                    {
                        var projectIdsFunders = unitWork.ProjectFundersRepository.GetProjection(f => model.OrganizationIds.Contains(f.FunderId), f => f.ProjectId);
                        var projectIdsImplementers = unitWork.ProjectImplementersRepository.GetProjection(f => model.OrganizationIds.Contains(f.ImplementerId), i => i.ProjectId);
                        var projectIdsList = projectIdsFunders.Union(projectIdsImplementers);

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.EndingFinancialYear.FinancialYear >= year),
                            new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.SectorIds.Count > 0)
                    {
                        var projectIds = unitWork.ProjectSectorsRepository.GetProjection(p => model.SectorIds.Contains(p.SectorId), p => p.ProjectId);
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIds.Contains(p.Id)
                            , new string[] { "StartingFinancialYear", "EndingFinancialYear", "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIds.Contains(project.Id)
                                                 select project;
                        }
                    }

                    if (model.StartingYear > 0 && model.EndingYear > 0)
                    {
                        financialYears = unitWork.FinancialYearRepository.GetProjection(y => (y.FinancialYear >= model.StartingYear && y.FinancialYear <= model.EndingYear), y => y.FinancialYear);
                    }
                    else if (model.StartingYear > 0 && model.EndingYear == 0)
                    {
                        financialYears = unitWork.FinancialYearRepository.GetProjection(y => (y.FinancialYear >= model.StartingYear), y => y.FinancialYear);
                    }
                    else
                    {
                        financialYears = unitWork.FinancialYearRepository.GetProjection(y => (y.FinancialYear != 0), y => y.FinancialYear);
                    }


                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    foreach (var project in projectProfileList)
                    {
                        decimal projectExchangeRate = (project.ExchangeRate == 0) ? 1 : project.ExchangeRate;
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.ProjectValue = (project.ProjectValue * (exchangeRate / projectExchangeRate));
                        profileView.ProjectCurrency = project.ProjectCurrency;
                        profileView.ExchangeRate = projectExchangeRate;
                        profileView.StartingFinancialYear = project.StartingFinancialYear.FinancialYear.ToString();
                        profileView.EndingFinancialYear = project.EndingFinancialYear.FinancialYear.ToString();
                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        projectsList.Add(profileView);
                    }

                    ProjectsByYear projectsByYear = null;
                    int totalYears = financialYears.Count();
                    List<ProjectsByYear> projectsListForYear = new List<ProjectsByYear>();
                    List<ProjectViewForYear> projectsViewForYear = new List<ProjectViewForYear>();
                    ICollection<YearWithProjects> yearProjects = new List<YearWithProjects>();

                    foreach (var yr in financialYears)
                    {
                        var isYearExists = (from y in yearProjects
                                            where y.Equals(yr)
                                            select y).FirstOrDefault();

                        if (isYearExists == null)
                        {
                            yearProjects.Add(new YearWithProjects()
                            {
                                Year = yr,
                                Projects = (from p in projectProfileList
                                            where yr >= p.StartingFinancialYear.FinancialYear && 
                                            (p.EndingFinancialYear.FinancialYear <= yr || p.EndingFinancialYear.FinancialYear > yr)
                                            select p.Id).ToList<int>()
                            });
                        }
                    }

                    foreach (var yearProject in yearProjects)
                    {
                        projectsByYear = new ProjectsByYear();
                        int currentYearId = yearProject.Year;

                        projectsViewForYear = new List<ProjectViewForYear>();
                        var projectIds = yearProject.Projects;
                        var yearlyProjectsProfile = (from project in projectsList
                                                     where projectIds.Contains(project.Id)
                                                     select project).ToList<ProjectProfileView>();

                        decimal totalFunding = 0, totalDisbursements = 0, totalProjectValue = 0, totalPlannedDisbursements = 0,
                            totalActualDisbursements = 0;
                        foreach (var project in yearlyProjectsProfile)
                        {
                            totalProjectValue += (project.ProjectValue * (exchangeRate / project.ExchangeRate));
                            //Disbursements
                            if (project.Disbursements.Count() > 0)
                            {
                                var disbursements = (from d in project.Disbursements
                                                     select d).ToList();

                                decimal projectDisbursements = Math.Round((from d in disbursements
                                                                           where d.DisbursementType == DisbursementTypes.Actual && d.Year == currentYearId
                                                                           select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);
                                decimal plannedDisbursements = Math.Round((from d in disbursements
                                                                           where d.DisbursementType == DisbursementTypes.Planned && d.Year == currentYearId
                                                                           select (d.Amount * (exchangeRate / d.ExchangeRate))).Sum(), MidpointRounding.AwayFromZero);
                                totalDisbursements += projectDisbursements;
                                UtilityHelper helper = new UtilityHelper();
                                project.ActualDisbursements = projectDisbursements;
                                project.PlannedDisbursements = plannedDisbursements;
                                if (project.PlannedDisbursements < 0)
                                {
                                    project.PlannedDisbursements = 0;
                                }
                                totalFunding += (project.ActualDisbursements + project.PlannedDisbursements);
                                totalPlannedDisbursements += project.PlannedDisbursements;
                                totalActualDisbursements += project.ActualDisbursements;
                            }

                            projectsViewForYear.Add(new ProjectViewForYear()
                            {
                                Title = project.Title.Replace("\"", ""),
                                StartingFinancialYear = project.StartingFinancialYear,
                                EndingFinancialYear = project.EndingFinancialYear,
                                Funders = string.Join(",", project.Funders.Select(f => f.Funder)),
                                Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                ProjectValue = (project.ProjectValue * (exchangeRate / project.ExchangeRate)),
                                ActualDisbursements = project.ActualDisbursements,
                                PlannedDisbursements = project.PlannedDisbursements,
                            });
                        }

                        if (projectsByYear != null)
                        {
                            projectsViewForYear = (from pv in projectsViewForYear
                                                   orderby pv.Title ascending
                                                   select pv).ToList();
                            projectsByYear.Year = currentYearId;
                            projectsByYear.TotalFunding = totalFunding;
                            projectsByYear.TotalDisbursements = totalDisbursements;
                            projectsByYear.TotalProjectValue = totalProjectValue;
                            projectsByYear.TotalActualDisbursements = totalActualDisbursements;
                            projectsByYear.TotalPlannedDisbursements = totalPlannedDisbursements;
                            projectsByYear.Projects = projectsViewForYear;
                            projectsListForYear.Add(projectsByYear);
                        }
                        else
                        {
                            projectsListForYear.Add(new ProjectsByYear()
                            {
                                Year = currentYearId,
                                Projects = new List<ProjectViewForYear>(),
                                TotalFunding = 0,
                                TotalDisbursements = 0,
                            });
                        }
                    }
                    timeSeriesReportByYear.YearlyProjectsList = projectsListForYear;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                return await Task<TimeSeriesReportByYear>.Run(() => timeSeriesReportByYear).ConfigureAwait(false);
            }
        }

        public decimal GetExchangeRateForCurrency(string currency, List<CurrencyWithRates> ratesList)
        {
            return (from rate in ratesList
                    where rate.Currency.Equals(currency)
                    select rate.Rate).FirstOrDefault();
        }

    }
}
