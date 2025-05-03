using System.ComponentModel;

namespace JobTrackingAPI.Enums;

public enum JobSource
{
    [Description("LinkedIn")]
    LinkedIn,
    Indeed,
    [Description("hiring.cafe")]
    HiringCafe,
    CompanyWebsite,
    JobBoard,
    Referral,
    Networking,
    RecruitmentAgency,
    SocialMedia,
    Email,
    Other
}
