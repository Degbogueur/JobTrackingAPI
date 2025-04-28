using FluentValidation;
using JobTrackingAPI.DTOs;

namespace JobTrackingAPI.Validators;

public class CreateApplicationValidator : BaseValidator<CreateApplicationDto>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.ApplicationDate)
            .NotEmpty()
                .WithMessage("Application date is required.")
            .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Application date must be today or in the past.")
            .Must(BeAValidDate)
                .WithMessage("Application date is not valid.");

        RuleFor(x => x.JobTitle)
            .NotEmpty()
                .WithMessage("Job title is required.")
            .MaximumLength(100)
                .WithMessage("Job title must not exceed 100 characters.");

        RuleFor(x => x.CompanyName)
            .NotEmpty()
                .WithMessage("Company name is required.")
            .MaximumLength(100)
                .WithMessage("Company name must not exceed 100 characters.");

        RuleFor(x => x.Location)
            .NotEmpty()
                .WithMessage("Location is required.")
            .MaximumLength(100)
                .WithMessage("Location must not exceed 100 characters.");

        RuleFor(x => x.Source)
            .IsInEnum()
                .WithMessage("Source is not valid.");

        RuleFor(x => x.ContractType)
            .IsInEnum()
                .WithMessage("Contract type is not valid.");

        RuleFor(x => x.OfferUrl)
            .NotEmpty()
                .WithMessage("Offer URL is required.")
            .Must(BeAValidUrl)
                .WithMessage("Offer URL is not valid.");

        RuleFor(x => x.PostingDate)
            .Must(BeAValidDate)
                .WithMessage("Posting date is not valid.")
            .When(x => x.PostingDate.HasValue);

        RuleFor(x => x.ClosingDate)
            .Must(BeAValidDate)
                .WithMessage("Closing date is not valid.")
            .When(x => x.ClosingDate.HasValue);

        RuleFor(x => x.Resume)
            .NotNull()
                .WithMessage("Resume file is required.")
            .Must(file => file.Length > 0)
                .WithMessage("Resume file must not be empty.")
            .Must(file => file.Length <= 10 * 1024 * 1024)
                .WithMessage("Resume file size must not exceed 10Mb.")
            .Must(file => file.ContentType == "application/pdf" || file.ContentType == "application/msword" || file.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                .WithMessage("Resume file type is not valid. Only PDF and Word documents are allowed.");

        RuleFor(x => x.CoverLetter)
            .Must(file => file == null || file.Length > 0)
                .WithMessage("Cover letter file must not be empty.")
            .Must(file => file == null || file.Length <= 10 * 1024 * 1024)
                .WithMessage("Cover letter file size must not exceed 10Mb.")
            .Must(file => file == null || file.ContentType == "application/pdf" || file.ContentType == "application/msword" || file.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                .WithMessage("Cover letter file type is not valid. Only PDF and Word documents are allowed.");

        RuleFor(x => x.Status)
            .IsInEnum()
                .WithMessage("Status is not valid.");

        RuleFor(x => x.LastAction)
            .IsInEnum()
                .WithMessage("Last action is not valid.");

        RuleFor(x => x.LastActionDate)
            .NotEmpty()
                .WithMessage("Last action date is required.")
            .Must(BeAValidDate)
                .WithMessage("Last action date is not valid.");

        RuleFor(x => x.NextAction)
            .IsInEnum()
                .WithMessage("Next action is not valid.");

        RuleFor(x => x.NextActionDate)
            .Must(BeAValidDate)
                .WithMessage("Next action date is not valid.")
            .When(x => x.NextActionDate.HasValue);

        RuleFor(x => x.Priority)
            .IsInEnum()
                .WithMessage("Priority is not valid.");

        RuleFor(x => new { x.MinSalaryProposed, x.MaxSalaryProposed })
            .Custom((salaries, context) =>
            {
                if (salaries.MaxSalaryProposed.HasValue && !salaries.MinSalaryProposed.HasValue)
                {
                    context.AddFailure("Minimum salary proposed must be provided if maximum salary proposed is provided.");
                }
                else if (salaries.MinSalaryProposed.HasValue && !salaries.MaxSalaryProposed.HasValue)
                {
                    context.AddFailure("Maximum salary proposed must be provided if minimum salary proposed is provided.");
                }
                else if (salaries.MinSalaryProposed.HasValue && salaries.MaxSalaryProposed.HasValue)
                {
                    if (salaries.MinSalaryProposed <= 0)
                    {
                        context.AddFailure("Minimum salary proposed must be greater than 0.");
                    }
                    if (salaries.MaxSalaryProposed < salaries.MinSalaryProposed)
                    {
                        context.AddFailure("Maximum salary proposed must be greater than or equal to minimum salary proposed.");
                    }
                }
            });

        RuleFor(x => new { x.MinSalaryOffered, x.MaxSalaryOffered })
            .Custom((salaries, context) =>
            {
                if (salaries.MaxSalaryOffered.HasValue && !salaries.MinSalaryOffered.HasValue)
                {
                    context.AddFailure("Minimum salary proposed must be provided if maximum salary proposed is provided.");
                }
                else if (salaries.MinSalaryOffered.HasValue && !salaries.MaxSalaryOffered.HasValue)
                {
                    context.AddFailure("Maximum salary proposed must be provided if minimum salary proposed is provided.");
                }
                else if (salaries.MinSalaryOffered.HasValue && salaries.MaxSalaryOffered.HasValue)
                {
                    if (salaries.MinSalaryOffered <= 0)
                    {
                        context.AddFailure("Minimum salary proposed must be greater than 0.");
                    }
                    if (salaries.MaxSalaryOffered < salaries.MinSalaryOffered)
                    {
                        context.AddFailure("Maximum salary proposed must be greater than or equal to minimum salary proposed.");
                    }
                }
            });

        RuleFor(x => x.Currency)
            .NotNull()
                .When(x => x.MinSalaryProposed.HasValue
                        || x.MaxSalaryProposed.HasValue
                        || x.MinSalaryOffered.HasValue
                        || x.MaxSalaryOffered.HasValue)
                .WithMessage("Currency is required if salary is provided.")
            .IsInEnum()
            .When(x => x.Currency.HasValue)
                .WithMessage("Currency is not valid.");

        RuleFor(x => x.KeyWords)
            .MaximumLength(300)
                .WithMessage("Keywords must not exceed 300 characters.")
            .When(x => !string.IsNullOrEmpty(x.KeyWords));

        RuleFor(x => x.InterestLevel)
            .InclusiveBetween(1, 5)
                .WithMessage("Interest level must be between 1 and 5.")
            .When(x => x.InterestLevel > 0);

        RuleFor(x => x.ContactName)
            .MaximumLength(100)
                .WithMessage("Contact name must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.ContactName));

        RuleFor(x => x.ContactEmail)
            .EmailAddress()
                .WithMessage("Contact email is not valid.")
            .When(x => !string.IsNullOrEmpty(x.ContactEmail));

        RuleFor(x => x.Notes)
            .MaximumLength(500)
                .WithMessage("Notes must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }    
}
