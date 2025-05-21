using System.Globalization;
using Humanizer;
using JobTrackingAPI.Contracts.Repositories;
using JobTrackingAPI.Contracts.Results;
using JobTrackingAPI.Contracts.Services;
using JobTrackingAPI.DTOs;
using JobTrackingAPI.Enums;
using JobTrackingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTrackingAPI.Services;

public class DashboardService(
    IApplicationRepository applicationRepository) : IDashboardService
{
    private readonly IApplicationRepository _applicationRepository = applicationRepository;

    private static readonly ApplicationStatus[] InProgressStatuses =
    [
        ApplicationStatus.Applied,
        ApplicationStatus.Viewed,
        ApplicationStatus.Shortlisted,
        ApplicationStatus.InterviewScheduled,
        ApplicationStatus.Interviewed,
        ApplicationStatus.OfferReceived
    ];

    private static readonly ApplicationStatus[] ResponseStatuses =
    [
        ApplicationStatus.Viewed,
        ApplicationStatus.Shortlisted,
        ApplicationStatus.InterviewScheduled,
        ApplicationStatus.Interviewed,
        ApplicationStatus.OfferReceived,
        ApplicationStatus.OfferAccepted,
        ApplicationStatus.OfferDeclined,
        ApplicationStatus.Rejected
    ];

    public async Task<Result<DashboardDto>> GetDashboardAsync(
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var applications = await GetAllApplicationsAsync(startDate, endDate);

        return Result<DashboardDto>.Success(new DashboardDto
        {
            TotalApplications = GetTotalApplications(applications),
            ApplicationsInProgress = GetApplicationsInProgress(applications),
            ResponseRate = GetApplicationsResponseRate(applications),
            AverageResponseTime = GetApplicationsAverageResponseTime(applications),
            StatusDistribution = GetApplicationsStatusDistribution(applications),
            MonthlyDistribution = GetApplicationsMonthlyDistribution(applications, startDate, endDate),
            SourceDistribution = GetApplicationsSourceDistribution(applications),
            ContractTypeDistribution = GetApplicationsContractTypeDistribution(applications),
            PriorityDistribution = GetApplicationsPriorityDistribution(applications),
            TopEnterprises = GetApplicationsTopEnterprises(applications),
            TopLocations = GetApplicationsTopLocations(applications),
            UpcomingActions = GetApplicationsUpcomingActions(applications),
            RecentApplications = GetRecentApplications(applications)
        });
    }

    private async Task<List<Application>> GetAllApplicationsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _applicationRepository.GetAll();
        return await query.Where(a => !a.IsDeleted &&
                                ((!startDate.HasValue && !endDate.HasValue) ||
                                a.ApplicationDate >= startDate &&
                                a.ApplicationDate <= endDate))
                          .ToListAsync();
    }

    private int GetTotalApplications(List<Application> applications)
    {
        return applications.Count;
    }

    private int GetApplicationsInProgress(List<Application> applications)
    {
        return applications.Count(a => InProgressStatuses.Contains(a.Status));
    }

    private double GetApplicationsResponseRate(List<Application> applications)
    {
        var totalCount = applications.Count;

        if (totalCount == 0)
            return 0.0;

        var respondedCount = applications.Count(a => ResponseStatuses.Contains(a.Status));

        var responseRate = (double)respondedCount / totalCount * 100;
        return Math.Round(responseRate, 2);
    }

    private double GetApplicationsAverageResponseTime(List<Application> applications)
    {
        var respondedApplications = applications.Where(a => a.FirstResponseDate != null);

        var count = respondedApplications.Count();
        if (count == 0)
            return 0.0;

        var totalDays = respondedApplications
            .Sum(a => (a.FirstResponseDate!.Value - a.ApplicationDate).TotalDays);

        var averageDays = (double)totalDays / count;

        return Math.Round(averageDays, 2);
    }

    private Dictionary<string, int> GetApplicationsStatusDistribution(List<Application> applications)
    {
        return applications
            .GroupBy(a => a.Status)
            .Select(a => new
            {
                Status = a.Key.Humanize(),
                Count = a.Count()
            })
            .ToDictionary(x => x.Status, x => x.Count);
    }

    private Dictionary<string, int> GetApplicationsMonthlyDistribution(
    List<Application> applications,
    DateTime? startDate,
    DateTime? endDate)
    {
        // Group applications by year and month, using two-digit year format
        var monthCounts = applications
            .GroupBy(a => new { a.ApplicationDate.Year, a.ApplicationDate.Month })
            .Select(a => new
            {
                YearMonth = $"{CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetAbbreviatedMonthName(a.Key.Month)} {a.Key.Year % 100:D2}",
                Count = a.Count()
            })
            .ToDictionary(x => x.YearMonth, x => x.Count);

        // Initialize all months in the range with zero counts
        var allMonths = new Dictionary<string, int>();

        if (startDate.HasValue && endDate.HasValue)
        {
            // Use provided date range
            var start = new DateTime(startDate.Value.Year, startDate.Value.Month, 1);
            var end = new DateTime(endDate.Value.Year, endDate.Value.Month, 1);
            for (var date = start; date <= end; date = date.AddMonths(1))
            {
                var key = $"{CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetAbbreviatedMonthName(date.Month)} {date.Year % 100:D2}";
                allMonths[key] = monthCounts.TryGetValue(key, out var count) ? count : 0;
            }
        }
        else
        {
            // Use range from earliest to latest ApplicationDate
            if (applications.Any())
            {
                var earliestDate = applications.Min(a => a.ApplicationDate);
                var latestDate = applications.Max(a => a.ApplicationDate);
                var start = new DateTime(earliestDate.Year, earliestDate.Month, 1);
                var end = new DateTime(latestDate.Year, latestDate.Month, 1);
                for (var date = start; date <= end; date = date.AddMonths(1))
                {
                    var key = $"{CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetAbbreviatedMonthName(date.Month)} {date.Year % 100:D2}";
                    allMonths[key] = monthCounts.TryGetValue(key, out var count) ? count : 0;
                }
            }
            // If no applications, return empty dictionary or handle as needed
        }

        return allMonths;
    }

    private Dictionary<string, int> GetApplicationsSourceDistribution(List<Application> applications)
    {
        return applications
            .GroupBy(a => a.Source)
            .Select(a => new
            {
                Source = a.Key.Humanize(),
                Count = a.Count()
            })
            .ToDictionary(x => x.Source, x => x.Count);
    }

    private Dictionary<string, int> GetApplicationsContractTypeDistribution(List<Application> applications)
    {
        return applications
            .GroupBy(a => a.ContractType)
            .Select(a => new
            {
                ContractType = a.Key.Humanize(),
                Count = a.Count()
            })
            .ToDictionary(x => x.ContractType, x => x.Count);
    }

    private Dictionary<string, int> GetApplicationsPriorityDistribution(List<Application> applications)
    {
        var allPriorities = new[] { Priority.Low, Priority.Medium, Priority.High, Priority.Critical };

        var priorityCounts = applications
            .GroupBy(a => a.Priority)
            .Select(a => new
            {
                Priority = a.Key,
                Count = a.Count()
            })
            .ToDictionary(x => x.Priority, x => x.Count);

        return allPriorities
            .ToDictionary(
                priority => priority.Humanize(),
                priority => priorityCounts.TryGetValue(priority, out var count) ? count : 0);
    }

    private Dictionary<string, int> GetApplicationsTopEnterprises(List<Application> applications)
    {
        return applications
            .GroupBy(a => a.CompanyName)
            .Select(a => new
            {
                CompanyName = a.Key,
                Count = a.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToDictionary(x => x.CompanyName, x => x.Count);
    }

    private Dictionary<string, int> GetApplicationsTopLocations(List<Application> applications)
    {
        return applications
            .GroupBy(a => a.Location)
            .Select(a => new
            {
                Location = a.Key,
                Count = a.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToDictionary(x => x.Location, x => x.Count);
    }

    private Dictionary<string, DateTime> GetApplicationsUpcomingActions(List<Application> applications)
    {
        return applications
            .Where(a => a.NextActionDate.HasValue)
            .OrderByDescending(a => a.NextActionDate!.Value)
            .Take(5)
            .ToDictionary(
                x => x.NextAction.Humanize(),
                x => x.NextActionDate!.Value);
    }

    private List<RecentApplication> GetRecentApplications(List<Application> applications)
    {
        return applications
            .OrderByDescending(a => a.ApplicationDate)
            .Take(5)
            .Select(a => new RecentApplication
            {
                Id = a.Id,
                ApplicationDateTime = a.ApplicationDate,
                CompanyName = a.CompanyName,
                JobTitle = a.JobTitle,
                Status = a.Status.Humanize()
            })
            .ToList();
    }
}