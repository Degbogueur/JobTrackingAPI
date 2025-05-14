using JobTrackingAPI.Contracts.Results;
using JobTrackingAPI.DTOs;
using JobTrackingAPI.Services;

namespace JobTrackingAPI.Contracts.Services;

public interface IApplicationService
{
    Task<Result<ApplicationDto>> CreateAsync(
        CreateApplicationDto createDto);

    Task<Result> DeleteAsync(int id);

    Task<Result<PaginatedResult<ApplicationDto>>> GetAllAsync(
        QueryParameters parameters);

    Task<Result<PaginatedResult<ApplicationDto>>> GetAllDeletedAsync(
        QueryParameters parameters);

    Task<Result<ApplicationDto>> GetByIdAsync(int id);

    Task<Result> RestoreAsync(int id);

    Task<Result> SoftDeleteAsync(int id);

    Task<Result<ApplicationDto>> UpdateAsync(
        int id,
        UpdateApplicationDto updateDto);

    Task<Result<ApplicationDto>> UpdateStatusAsync(
        int id, 
        UpdateApplicationStatusDto updateStatusDto);
}