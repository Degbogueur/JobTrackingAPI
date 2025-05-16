using JobTrackingAPI.Models;

namespace JobTrackingAPI.Contracts.Repositories;

public interface IApplicationRepository
{
    Task<Application> CreateAsync(
        Application application);

    void Delete(Application application);

    Task<bool> ExistsAsync(
        string jobTitle,
        string companyName,
        string location,
        int? excludedId = null);

    IQueryable<Application> GetAll();

    Task<Application?> GetByIdAsync(
        int id);

    Task SaveChangesAsync();
}