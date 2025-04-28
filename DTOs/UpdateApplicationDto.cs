using JobTrackingAPI.Enums;

namespace JobTrackingAPI.DTOs;

public class UpdateApplicationDto
{
    public DateTime ApplicationDate { get; set; }
    public string JobTitle { get; set; }
    public string? JobDescription { get; set; }
    public string CompanyName { get; set; }
    public string Location { get; set; }
    public JobSource Source { get; set; }
    public ContractType ContractType { get; set; }
    public string OfferUrl { get; set; }
    public DateTime? PostingDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public IFormFile? Resume { get; set; }
    public IFormFile? CoverLetter { get; set; }
    public ApplicationStatus Status { get; set; }
    public ActionType LastAction { get; set; }
    public DateTime LastActionDate { get; set; }
    public ActionType NextAction { get; set; }
    public DateTime? NextActionDate { get; set; }
    public Priority Priority { get; set; }
    public string? Notes { get; set; }
    public decimal? MinSalaryProposed { get; set; }
    public decimal? MaxSalaryProposed { get; set; }
    public decimal? MinSalaryOffered { get; set; }
    public decimal? MaxSalaryOffered { get; set; }
    public Currency? Currency { get; set; }
    public RejectionReason? RejectionReason { get; set; }
    public string? KeyWords { get; set; }
    public int InterestLevel { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
}