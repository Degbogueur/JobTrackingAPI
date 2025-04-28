using Humanizer;
using JobTrackingAPI.Contracts.Results;
using JobTrackingAPI.Contracts.Services;

namespace JobTrackingAPI.Services;

public class LocalFileService(
    ILogger<LocalFileService> logger) : IFileService
{
    private readonly ILogger<LocalFileService> _logger = logger;
    private readonly string _basePath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Uploads");

    public Task DeleteFileAsync(string filePath)
    {
        var absolutePath = Path.Combine(_basePath, filePath);

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }
        else
        {
            _logger.LogWarning("File {FilePath} not found.", filePath);
        }

        return Task.CompletedTask;
    }

    public async Task<Result<string>> UploadCoverLetterFileAsync(
        IFormFile coverLetter,
        string companyName,
        string jobTitle,
        string location)
    {
        return await UploadFileAsync(
            coverLetter,
            companyName,
            jobTitle,
            location,
            "CoverLetter");
    }

    public async Task<Result<string>> UploadOtherFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("Attempted to upload an empty file.");
            return Result<string>.Failure(
                Enums.ErrorType.FileUploadError,
                "File is empty.");
        }

        var folderPath = Path.Combine(_basePath, "OtherFiles");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Result<string>.Success(GetRelativePath(filePath));
    }

    public async Task<Result<string>> UploadResumeFileAsync(
        IFormFile resume,
        string companyName,
        string jobTitle,
        string location)
    {
        return await UploadFileAsync(
            resume,
            companyName,
            jobTitle,
            location,
            "Resume");
    }

    private string GetApplicationFolderPath(
        string companyName,
        string jobTitle,
        string location)
    {
        var folderPath = Path.Combine(
            _basePath,
            "Applications",
            $"{SanitizeName(companyName)}_{SanitizeName(jobTitle)}_{SanitizeName(location)}");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        return folderPath;
    }

    private string GetRelativePath(
        string absolutePath)
    {
        return Path.GetRelativePath(
            _basePath,
            absolutePath);
    }

    private string SanitizeName(string name)
    {
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }

        return name.Replace(" ", "_");
    }

    private async Task<Result<string>> UploadFileAsync(
        IFormFile file,
        string companyName,
        string jobTitle,
        string location,
        string prefix)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("{Prefix} file is empty.", prefix.Humanize());
            return Result<string>.Failure(
                Enums.ErrorType.FileUploadError,
                $"{prefix.Humanize()} file is empty.");
        }

        var folderPath = GetApplicationFolderPath(companyName, jobTitle, location);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{prefix}_{SanitizeName(companyName)}_{SanitizeName(jobTitle)}_{SanitizeName(location)}{extension}";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Result<string>.Success(GetRelativePath(filePath));
    }
}