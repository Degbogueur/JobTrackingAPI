using JobTrackingAPI.Contracts.Repositories;
using JobTrackingAPI.Contracts.Results;
using JobTrackingAPI.Contracts.Services;
using JobTrackingAPI.DTOs;
using JobTrackingAPI.Mappers;
using JobTrackingAPI.Validators;
using Microsoft.EntityFrameworkCore;

namespace JobTrackingAPI.Services;

public class ApplicationService(
    IApplicationRepository applicationRepository,
    IFileService fileService) : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository = applicationRepository;
    private readonly IFileService _fileService = fileService;

    public async Task<Result<ApplicationDto>> CreateAsync(
        CreateApplicationDto createDto)
    {
        bool alreadyExists = await _applicationRepository.ExistsAsync(
            createDto.JobTitle,
            createDto.CompanyName,
            createDto.Location
        );

        if (alreadyExists)
        {
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.Conflict,
                "An application with the same job title, company name, and location already exists.");
        }


        var validator = new CreateApplicationValidator();
        var validationResult = await validator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.ValidationError,
                string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var application = createDto.ToModel();

        var resumeFileResult = await _fileService.UploadResumeFileAsync(
            createDto.Resume,
            createDto.CompanyName,
            createDto.JobTitle,
            createDto.Location);

        if (resumeFileResult.IsFailure)
        {
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.FileUploadError,
                resumeFileResult.ErrorMessage!);
        }

        application.ResumeFilePath = resumeFileResult.Data;

        if (createDto.CoverLetter != null)
        {
            var coverLetterFileResult = await _fileService.UploadCoverLetterFileAsync(
                createDto.CoverLetter,
                createDto.CompanyName,
                createDto.JobTitle,
                createDto.Location);

            if (coverLetterFileResult.IsFailure)
            {
                return Result<ApplicationDto>.Failure(
                    Enums.ErrorType.FileUploadError,
                    coverLetterFileResult.ErrorMessage!);
            }

            application.CoverLetterFilePath = coverLetterFileResult.Data;
        }

        application = await _applicationRepository.CreateAsync(application);
        await _applicationRepository.SaveChangesAsync();

        return Result<ApplicationDto>.Success(
            application.ToDto());
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);

        if (application is null)
        {
            return Result.Failure(
                Enums.ErrorType.NotFound,
                "Application not found.");
        }

        application.IsDeleted = true;
        await _applicationRepository.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<PaginatedResult<ApplicationDto>>> GetAllAsync(
        int pageIndex = 1,
        int pageSize = 20)
    {
        var totalCount = await _applicationRepository
            .GetAllAsync()
            .CountAsync();

        var applications = await _applicationRepository.GetAllAsync()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(a => a.ToDto())
            .AsNoTracking()
            .ToListAsync();
        
        var paginatedResult = PaginatedResult<ApplicationDto>.Create(
            applications,
            totalCount,
            pageIndex,
            pageSize);

        return Result<PaginatedResult<ApplicationDto>>.Success(
            paginatedResult);
    }

    public async Task<Result<ApplicationDto>> GetByIdAsync(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);

        if (application is null)
        {
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.NotFound,
                "Application not found.");
        }

        return Result<ApplicationDto>.Success(
            application.ToDto());
    }

    public async Task<Result> RestoreAsync(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);

        if (application is null)
        {
            return Result.Failure(
                Enums.ErrorType.NotFound,
                "Application not found.");
        }

        application.IsDeleted = false;
        await _applicationRepository.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<ApplicationDto>> UpdateAsync(
        int id, 
        UpdateApplicationDto updateDto)
    {
        var application = await _applicationRepository.GetByIdAsync(id);

        if (application is null)
        {
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.NotFound,
                "Application not found.");
        }

        bool alreadyExists = await _applicationRepository.ExistsAsync(
            updateDto.JobTitle,
            updateDto.CompanyName,
            updateDto.Location,
            excludedId: id
        );

        if (alreadyExists)
        {
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.Conflict,
                "An application with the same job title, company name, and location already exists.");
        }

        var validator = new UpdateApplicationValidator();
        var validationResult = await validator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.ValidationError,
                string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        application.UpdateFromDto(updateDto);

        if (updateDto.Resume != null)
        {
            var resumeFileResult = await _fileService.UploadResumeFileAsync(
                updateDto.Resume,
                updateDto.CompanyName,
                updateDto.JobTitle,
                updateDto.Location);

            if (resumeFileResult.IsFailure)
            {
                return Result<ApplicationDto>.Failure(
                    Enums.ErrorType.FileUploadError,
                    resumeFileResult.ErrorMessage!);
            }

            application.ResumeFilePath = resumeFileResult.Data;
        }

        if (updateDto.CoverLetter != null)
        {
            var coverLetterFileResult = await _fileService.UploadCoverLetterFileAsync(
                updateDto.CoverLetter,
                updateDto.CompanyName,
                updateDto.JobTitle,
                updateDto.Location);

            if (coverLetterFileResult.IsFailure)
            {
                return Result<ApplicationDto>.Failure(
                    Enums.ErrorType.FileUploadError,
                    coverLetterFileResult.ErrorMessage!);
            }

            application.CoverLetterFilePath = coverLetterFileResult.Data;
        }

        await _applicationRepository.SaveChangesAsync();

        return Result<ApplicationDto>.Success(
            application.ToDto());
    }
}