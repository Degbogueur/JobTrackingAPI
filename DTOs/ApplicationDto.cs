using JobTrackingAPI.Enums;

namespace JobTrackingAPI.DTOs;

public class ApplicationDto
{
    public int Id { get; set; }
    public DateTime ApplicationDate { get; set; }
    public string JobTitle { get; set; }
    public string? JobDescription { get; set; }
    public string CompanyName { get; set; }
    public string Location { get; set; }
    public string Source { get; set; }
    public string ContractType { get; set; }
    public string OfferUrl { get; set; }
    public DateTime? PostingDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public string ResumeFilePath { get; set; }
    public string? CoverLetterFilePath { get; set; }
    public string Status { get; set; }
    public string LastAction { get; set; }
    public DateTime LastActionDate { get; set; }
    public string NextAction { get; set; }
    public DateTime? NextActionDate { get; set; }
    public string Priority { get; set; }
    public string? Notes { get; set; }
    public decimal? MinSalaryProposed { get; set; }
    public decimal? MaxSalaryProposed { get; set; }
    public decimal? MinSalaryOffered { get; set; }
    public decimal? MaxSalaryOffered { get; set; }
    public string? Currency { get; set; }
    public string? RejectionReason { get; set; }
    public string? KeyWords { get; set; }
    public int InterestLevel { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
}
