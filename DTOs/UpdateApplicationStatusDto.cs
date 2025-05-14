using JobTrackingAPI.Enums;

namespace JobTrackingAPI.DTOs;

public class UpdateApplicationStatusDto
{
    public ApplicationStatus Status { get; set; }
    public RejectionReason? RejectionReason { get; set; }
    public ActionType NextAction { get; set; }
    public DateTime? NextActionDate { get; set; }
}
