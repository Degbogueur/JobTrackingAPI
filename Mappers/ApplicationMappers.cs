using Humanizer;
using JobTrackingAPI.DTOs;
using JobTrackingAPI.Models;

namespace JobTrackingAPI.Mappers;

public static class ApplicationMappers
{
    public static Application ToModel(this CreateApplicationDto createDto)
    {
        return new Application
        {
            ApplicationDate = createDto.ApplicationDate,
            JobTitle = createDto.JobTitle,
            JobDescription = createDto.JobDescription,
            CompanyName = createDto.CompanyName,
            Location = createDto.Location,
            Source = createDto.Source,
            ContractType = createDto.ContractType,
            OfferUrl = createDto.OfferUrl,
            PostingDate = createDto.PostingDate,
            ClosingDate = createDto.ClosingDate,
            Status = createDto.Status,
            LastAction = createDto.LastAction,
            LastActionDate = createDto.LastActionDate,
            NextAction = createDto.NextAction,
            NextActionDate = createDto.NextActionDate,
            Priority = createDto.Priority ?? Enums.Priority.Medium,
            Notes = createDto.Notes,
            MinSalaryProposed = createDto.MinSalaryProposed,
            MaxSalaryProposed = createDto.MaxSalaryProposed,
            MinSalaryOffered = createDto.MinSalaryOffered,
            MaxSalaryOffered = createDto.MaxSalaryOffered,
            Currency = createDto.Currency,
            KeyWords = createDto.KeyWords,
            InterestLevel = createDto.InterestLevel,
            ContactName = createDto.ContactName,
            ContactEmail = createDto.ContactEmail
        };
    }

    public static ApplicationDto ToDto(this Application application)
    {
        return new ApplicationDto
        {
            Id = application.Id,
            ApplicationDate = application.ApplicationDate,
            JobTitle = application.JobTitle,
            JobDescription = application.JobDescription,
            CompanyName = application.CompanyName,
            Location = application.Location,
            Source = application.Source.Humanize(),
            ContractType = application.ContractType.Humanize(),
            OfferUrl = application.OfferUrl,
            PostingDate = application.PostingDate,
            ClosingDate = application.ClosingDate,
            ResumeFileName = Path.GetFileName(application.ResumeFilePath),
            ResumeFilePath = application.ResumeFilePath,
            CoverLetterFileName = Path.GetFileName(application.CoverLetterFilePath),
            CoverLetterFilePath = application.CoverLetterFilePath,
            Status = application.Status.Humanize(),
            LastAction = application.LastAction.Humanize(),
            LastActionDate = application.LastActionDate,
            NextAction = application.NextAction.Humanize(),
            NextActionDate = application.NextActionDate,
            Priority = application.Priority.Humanize(),
            Notes = application.Notes,
            MinSalaryProposed = application.MinSalaryProposed,
            MaxSalaryProposed = application.MaxSalaryProposed,
            MinSalaryOffered = application.MinSalaryOffered,
            MaxSalaryOffered = application.MaxSalaryOffered,
            Currency = application.Currency?.Humanize(),
            RejectionReason = application.RejectionReason?.Humanize(),
            KeyWords = application.KeyWords,
            InterestLevel = application.InterestLevel,
            ContactName = application.ContactName,
            ContactEmail = application.ContactEmail
        };
    }

    public static void UpdateFromDto(this Application application,
        UpdateApplicationDto updateDto)
    {
        application.ApplicationDate = updateDto.ApplicationDate;
        application.JobTitle = updateDto.JobTitle;
        application.JobDescription = updateDto.JobDescription;
        application.CompanyName = updateDto.CompanyName;
        application.Location = updateDto.Location;
        application.Source = updateDto.Source;
        application.ContractType = updateDto.ContractType;
        application.OfferUrl = updateDto.OfferUrl;
        application.PostingDate = updateDto.PostingDate;
        application.ClosingDate = updateDto.ClosingDate;
        application.Status = updateDto.Status;
        application.LastAction = updateDto.LastAction;
        application.LastActionDate = updateDto.LastActionDate;
        application.NextAction = updateDto.NextAction;
        application.NextActionDate = updateDto.NextActionDate;
        application.Priority = updateDto.Priority;
        application.Notes = updateDto.Notes;
        application.MinSalaryProposed = updateDto.MinSalaryProposed;
        application.MaxSalaryProposed = updateDto.MaxSalaryProposed;
        application.MinSalaryOffered = updateDto.MinSalaryOffered;
        application.MaxSalaryOffered = updateDto.MaxSalaryOffered;
        application.Currency = updateDto.Currency;
        application.RejectionReason = updateDto.RejectionReason;
        application.KeyWords = updateDto.KeyWords;
        application.InterestLevel = updateDto.InterestLevel;
        application.ContactName = updateDto.ContactName;
        application.ContactEmail = updateDto.ContactEmail;
    }
}
