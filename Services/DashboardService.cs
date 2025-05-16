using System.Threading.Tasks;
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
            AverageResponseTime = await GetApplicationsAverageResponseTimeAsync()
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
}