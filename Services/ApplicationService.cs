﻿using JobTrackingAPI.Contracts.Repositories;
using JobTrackingAPI.Contracts.Results;
using JobTrackingAPI.Contracts.Services;
using JobTrackingAPI.DTOs;
using JobTrackingAPI.Enums;
using JobTrackingAPI.Mappers;
using JobTrackingAPI.Models;
using JobTrackingAPI.Validators;
using Microsoft.EntityFrameworkCore;

namespace JobTrackingAPI.Services;

public class ApplicationService(
    IApplicationRepository applicationRepository,
    IFileService fileService,
    ILogger<ApplicationService> logger) : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository = applicationRepository;
    private readonly IFileService _fileService = fileService;
    private readonly ILogger<ApplicationService> _logger = logger;

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
            _logger.LogError(
                "An application with the same job title, company name, and location already exists.");
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.Conflict,
                "An application with the same job title, company name, and location already exists.");
        }


        var validator = new CreateApplicationValidator();
        var validationResult = await validator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            _logger.LogError(
                "Validation failed for CreateApplicationDto: {Errors}",
                string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
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
            _logger.LogError(
                "Failed to upload resume file: {ErrorMessage}",
                resumeFileResult.ErrorMessage);
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
                _logger.LogError(
                    "Failed to upload cover letter file: {ErrorMessage}",
                    coverLetterFileResult.ErrorMessage);
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
            _logger.LogError(
                "Application with ID {Id} not found.",
                id);
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.NotFound,
                "Application not found.");
        }

        _applicationRepository.Delete(application);
        await _applicationRepository.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<PaginatedResult<ApplicationDto>>> GetAllAsync(
        QueryParameters parameters)
    {
        var paginatedResult = await GetAllApplicationsAsync(parameters);

        return Result<PaginatedResult<ApplicationDto>>.Success(
            paginatedResult);
    }

    public async Task<Result<PaginatedResult<ApplicationDto>>> GetAllDeletedAsync(
        QueryParameters parameters)
    {
        var paginatedResult = await GetAllApplicationsAsync(
            parameters,
            isDeleted: true);

        return Result<PaginatedResult<ApplicationDto>>.Success(
            paginatedResult);
    }

    public async Task<Result<ApplicationDto>> GetByIdAsync(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);

        if (application is null)
        {
            _logger.LogError(
                "Application with ID {Id} not found.",
                id);
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
            _logger.LogError(
                "Application with ID {Id} not found.",
                id);
            return Result.Failure(
                Enums.ErrorType.NotFound,
                "Application not found.");
        }

        application.IsDeleted = false;
        await _applicationRepository.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> SoftDeleteAsync(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);

        if (application is null)
        {
            _logger.LogError(
                "Application with ID {Id} not found.",
                id);
            return Result.Failure(
                Enums.ErrorType.NotFound,
                "Application not found.");
        }

        application.IsDeleted = true;
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
            _logger.LogError(
                "Application with ID {Id} not found.",
                id);
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
            _logger.LogError(
                "An application with the same job title, company name, and location already exists.");
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.Conflict,
                "An application with the same job title, company name, and location already exists.");
        }

        var validator = new UpdateApplicationValidator();
        var validationResult = await validator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            _logger.LogError(
                "Validation failed for UpdateApplicationDto: {Errors}",
                string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.ValidationError,
                string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        UpdateFirstResponseDate(application, updateDto.Status);

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
                _logger.LogError(
                    "Failed to upload resume file: {ErrorMessage}",
                    resumeFileResult.ErrorMessage);
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
                _logger.LogError(
                    "Failed to upload cover letter file: {ErrorMessage}",
                    coverLetterFileResult.ErrorMessage);
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

    public async Task<Result<ApplicationDto>> UpdateStatusAsync(
        int id,
        UpdateApplicationStatusDto updateStatusDto)
    {
        var application = await _applicationRepository.GetByIdAsync(id);

        if (application is null)
        {
            _logger.LogError(
                "Application with ID {Id} not found.",
                id);
            return Result<ApplicationDto>.Failure(
                Enums.ErrorType.NotFound,
                "Application not found.");
        }

        UpdateFirstResponseDate(application, updateStatusDto.Status);

        application.UpdateStatusFromDto(updateStatusDto);
        await _applicationRepository.SaveChangesAsync();

        return Result<ApplicationDto>.Success(
            application.ToDto());
    }

    private async Task<PaginatedResult<ApplicationDto>> GetAllApplicationsAsync(
        QueryParameters parameters, 
        bool isDeleted = false)
    {
        var pageIndex = parameters.PageIndex;
        var pageSize = parameters.PageSize;
        var searchTerm = parameters.SearchTerm;
        var sortBy = parameters.SortBy;

        var query = _applicationRepository
            .GetAll()
            .Where(a => (string.IsNullOrEmpty(searchTerm) ||
                        a.JobTitle.Contains(searchTerm) ||
                        a.CompanyName.Contains(searchTerm) ||
                        a.Location.Contains(searchTerm)) &&
                        a.IsDeleted == isDeleted);

        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "name-asc" => query.OrderBy(a => a.JobTitle),
                "name-desc" => query.OrderByDescending(a => a.JobTitle),
                "date-asc" => query.OrderBy(a => a.ApplicationDate),
                "date-desc" => query.OrderByDescending(a => a.ApplicationDate),
                _ => query.OrderByDescending(a => a.ApplicationDate)
            };
        }
        else
        {
            query = query.OrderByDescending(a => a.ApplicationDate);
        }

        if (parameters.Statuses != null)
        {
            query = query.Where(a => parameters.Statuses.Contains(a.Status));
        }

        if (parameters.Priorities != null)
        {
            query = query.Where(a => parameters.Priorities.Contains(a.Priority));
        }

        var totalCount = await query.CountAsync();

        var applications = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(a => a.ToDto())
            .AsNoTracking()
            .ToListAsync();

        return PaginatedResult<ApplicationDto>.Create(
            applications,
            totalCount,
            pageIndex,
            pageSize);
    }

    private void UpdateFirstResponseDate(Application application, ApplicationStatus status)
    {
        var isResponseStatus = new[]
        {
            ApplicationStatus.Viewed,
            ApplicationStatus.Shortlisted,
            ApplicationStatus.InterviewScheduled,
            ApplicationStatus.Interviewed,
            ApplicationStatus.OfferReceived,
            ApplicationStatus.OfferAccepted,
            ApplicationStatus.OfferDeclined,
            ApplicationStatus.Rejected
        }.Contains(status);

        if (application.FirstResponseDate is null && isResponseStatus)
        {
            application.FirstResponseDate = DateTime.Today;
        }
    }
}

public class QueryParameters
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 9;
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public string? StatusFilters { get; set; }
    public string? PriorityFilters { get; set; }

    public ApplicationStatus[]? Statuses =>
        StatusFilters?.Split(',')
            .Select(s => Enum.TryParse<ApplicationStatus>(s, true, out var status) ? status : default)
            .ToArray();

    public Priority[]? Priorities =>
        PriorityFilters?.Split(',')
            .Select(s => Enum.TryParse<Priority>(s, true, out var priority) ? priority : default)
            .ToArray();
}