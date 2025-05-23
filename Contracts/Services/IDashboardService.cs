﻿using JobTrackingAPI.Contracts.Results;
using JobTrackingAPI.DTOs;

namespace JobTrackingAPI.Contracts.Services;

public interface IDashboardService
{
    Task<Result<DashboardDto>> GetDashboardAsync(
        DateTime? startDate = null,
        DateTime? endDate = null);
}
