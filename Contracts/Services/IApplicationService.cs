using JobTrackingAPI.Contracts.Results;
using JobTrackingAPI.DTOs;

namespace JobTrackingAPI.Contracts.Services;

public interface IApplicationService
{
    Task<Result<ApplicationDto>> CreateAsync(
        CreateApplicationDto createDto);

    Task<Result> DeleteAsync(int id);

    Task<Result<PaginatedResult<ApplicationDto>>> GetAllAsync(
        int pageIndex = 1,
        int pageSize = 20);

    Task<Result<ApplicationDto>> GetByIdAsync(int id);

    Task<Result> RestoreAsync(int id);

    Task<Result<ApplicationDto>> UpdateAsync(
        int id,
        UpdateApplicationDto updateDto);
}