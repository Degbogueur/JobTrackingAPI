using JobTrackingAPI.Contracts.Results;

namespace JobTrackingAPI.Contracts.Services;

public interface IFileService
{
    Task<Result<string>> UploadResumeFileAsync(
        IFormFile resume,
        string companyName,
        string jobTitle,
        string location
    );

    Task<Result<string>> UploadCoverLetterFileAsync(
        IFormFile coverLetter,
        string companyName,
        string jobTitle,
        string location
    );

    Task<Result<string>> UploadOtherFileAsync(
        IFormFile file
    );

    Task DeleteFileAsync(
        string filePath
    );
}
