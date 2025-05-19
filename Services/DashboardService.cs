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

    public async Task<Result<DashboardDto>> GetDashboardAsync()
    {
        return Result<DashboardDto>.Success(new DashboardDto
        {
            TotalApplications = await GetTotalApplicationsAsync(),
            ApplicationsInProgress = await GetApplicationsInProgressAsync(),
            ResponseRate = await GetApplicationsResponseRateAsync(),
            AverageResponseTime = await GetApplicationsAverageResponseTimeAsync(),
            StatusDistribution = await GetApplicationsStatusDistributionAsync(),
            MonthlyDistribution = await GetApplicationsMonthlyDistributionAsync(),
            SourceDistribution = await GetApplicationsSourceDistributionAsync(),
            ContractTypeDistribution = await GetApplicationsContractTypeDistributionAsync(),
            PriorityDistribution = await GetApplicationsPriorityDistributionAsync(),
            TopEnterprises = await GetApplicationsTopEnterprisesAsync(),
            TopLocations = await GetApplicationsTopLocationsAsync(),
            NextActions = await GetApplicationsNextActionsAsync(),
            RecentApplications = await GetRecentApplicationsAsync()
        });
    }    

    private IQueryable<Application> GetAll()
    {
        var query = _applicationRepository.GetAll();
        return query.Where(a => !a.IsDeleted);
    }

    private async Task<int> GetTotalApplicationsAsync()
    {
        var applications = GetAll();
        return await applications.CountAsync();
    }

    private async Task<int> GetApplicationsInProgressAsync()
    {
        var applications = GetAll();
        return await applications.CountAsync(a => InProgressStatuses.Contains(a.Status));
    }

    private async Task<double> GetApplicationsResponseRateAsync()
    {
        var applications = GetAll();
        var totalCount = await applications.CountAsync();

        if (totalCount == 0)
            return 0.0;

        var respondedCount =await applications.CountAsync(a => ResponseStatuses.Contains(a.Status));

        var responseRate = (double)respondedCount / totalCount * 100;
        return Math.Round(responseRate, 2);
    }

    private async Task<double> GetApplicationsAverageResponseTimeAsync()
    {
        var applications = GetAll();
        var respondedApplications = applications.Where(a => a.FirstResponseDate != null);

        var count = await respondedApplications.CountAsync();
        if (count == 0)
            return 0.0;

        var totalDays = await respondedApplications
            .SumAsync(a => (a.FirstResponseDate!.Value - a.ApplicationDate).TotalDays);

        var averageDays = (double)totalDays / count;

        return Math.Round(averageDays, 2);
    }

    private async Task<Dictionary<string, int>> GetApplicationsStatusDistributionAsync()
    {
        var applications = GetAll();
        return await applications
            .GroupBy(a => a.Status)
            .Select(a => new
            {
                Status = a.Key.Humanize(),
                Count = a.Count()
            })
            .ToDictionaryAsync(
                x => x.Status,
                x => x.Count);
    }

    private async Task<Dictionary<string, int>> GetApplicationsMonthlyDistributionAsync()
    {
        var applications = GetAll();
        var monthCounts = await applications
            .GroupBy(a => a.ApplicationDate.Month)
            .Select(a => new
            {
                Month = a.Key,
                Count = a.Count()
            })
            .ToDictionaryAsync(x => x.Month, x => x.Count);

        var allMonths = Enumerable.Range(1, 12)
            .ToDictionary(
                month => CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetAbbreviatedMonthName(month),
                month => monthCounts.TryGetValue(month, out var count) ? count : 0);

        return allMonths;
    }

    private async Task<Dictionary<string, int>> GetApplicationsSourceDistributionAsync()
    {
        var applications = GetAll();
        return await applications
            .GroupBy(a => a.Source)
            .Select(a => new
            {
                Source = a.Key.Humanize(),
                Count = a.Count()
            })
            .ToDictionaryAsync(
                x => x.Source,
                x => x.Count);
    }

    private async Task<Dictionary<string, int>> GetApplicationsContractTypeDistributionAsync()
    {
        var applications = GetAll();
        return await applications
            .GroupBy(a => a.ContractType)
            .Select(a => new
            {
                ContractType = a.Key.Humanize(),
                Count = a.Count()
            })
            .ToDictionaryAsync(
                x => x.ContractType,
                x => x.Count);
    }

    private async Task<Dictionary<string, int>> GetApplicationsPriorityDistributionAsync()
    {
        var applications = GetAll();
        return await applications
            .GroupBy(a => a.Priority)
            .Select(a => new
            {
                Priority = a.Key.Humanize(),
                Count = a.Count()
            })
            .ToDictionaryAsync(
                x => x.Priority,
                x => x.Count);
    }

    private async Task<Dictionary<string, int>> GetApplicationsTopEnterprisesAsync()
    {
        var applications = GetAll();
        return await applications
            .GroupBy(a => a.CompanyName)
            .Select(a => new
            {
                CompanyName = a.Key,
                Count = a.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToDictionaryAsync(
                x => x.CompanyName,
                x => x.Count);
    }

    private async Task<Dictionary<string, int>> GetApplicationsTopLocationsAsync()
    {
        var applications = GetAll();
        return await applications
            .GroupBy(a => a.Location)
            .Select(a => new
            {
                Location = a.Key,
                Count = a.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToDictionaryAsync(
                x => x.Location,
                x => x.Count);
    }

    private async Task<List<NextAction>> GetApplicationsNextActionsAsync()
    {
        var applications = GetAll();
        return await applications
            .Where(a => a.NextActionDate.HasValue)
            .OrderByDescending(a => a.NextActionDate!.Value)
            .Select(a => new NextAction
            {
                ActionName = a.NextAction.Humanize(),
                ActionDate = a.NextActionDate
            })
            .ToListAsync();
    }

    private async Task<List<RecentApplication>> GetRecentApplicationsAsync()
    {
        var applications = GetAll();
        return await applications
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
            .ToListAsync();
    }
}