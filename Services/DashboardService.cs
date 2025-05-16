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

    public async Task<Result<DashboardDto>> GetDashboardAsync()
    {
        return Result<DashboardDto>.Success(
            new DashboardDto
            {
                TotalApplications = await GetTotalApplicationsAsync(),
                ApplicationsInProgress = await GetApplicationsInProgressAsync(),
                ResponseRate = await GetApplicationsResponseRateAsync(),
            });
    }

    private async Task<List<Application>> GetAllAsync()
    {
        var query = _applicationRepository.GetAllAsync();
        return await query.Where(a => !a.IsDeleted).ToListAsync();
    }

    private async Task<List<Application>> GetRespondedApplicationsAsync()
    {
        var applications = await GetAllAsync();
        return applications
            .Where(a => !new[]
            {
                ApplicationStatus.Draft,
                ApplicationStatus.Applied,
                ApplicationStatus.NoResponse,
                ApplicationStatus.Withdrawn,
                ApplicationStatus.NotInterested
            }.Contains(a.Status))
            .ToList();
    }

    private async Task<int> GetTotalApplicationsAsync()
    {
        var applications = await GetAllAsync();
        return applications.Count;
    }

    private async Task<int> GetApplicationsInProgressAsync()
    {
        var applications = await GetAllAsync();
        return applications.Count(a => new[]
        {
            ApplicationStatus.Applied,
            ApplicationStatus.Viewed,
            ApplicationStatus.Shortlisted,
            ApplicationStatus.InterviewScheduled,
            ApplicationStatus.Interviewed,
            ApplicationStatus.OfferReceived
        }.Contains(a.Status));
    }

    private async Task<double> GetApplicationsResponseRateAsync()
    {
        var applications = await GetAllAsync();

        if (applications.Count == 0)
            return 0.0;

        var respondedApplications = await GetRespondedApplicationsAsync();

        var responseRate = (double)respondedApplications.Count / applications.Count * 100;
        return Math.Round(responseRate, 2);
    }

    
}
