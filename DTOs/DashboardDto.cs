namespace JobTrackingAPI.DTOs;

public class DashboardDto
{
    public int TotalApplications { get; set; }
    public int ApplicationsInProgress { get; set; }
    public double ResponseRate { get; set; }
    public double AverageResponseTime { get; set; }

    public Dictionary<string, int> StatusDistribution { get; set; } = [];

    public Dictionary<string, int> MonthlyDistribution { get; set; } = [];

    public Dictionary<string, int> SourceDistribution { get; set; } = [];

    public Dictionary<string, int> ContractTypeDistribution { get; set; } = [];

    public Dictionary<string, int> PriorityDistribution { get; set; } = [];

    public Dictionary<string, int> TopEnterprises { get; set; } = [];

    public Dictionary<string, int> TopLocations { get; set; } = [];

    public Dictionary<string, DateTime> UpcomingActions { get; set; } = [];

    public List<RecentApplication> RecentApplications { get; set; } = [];
}

public class RecentApplication
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public DateTime ApplicationDateTime { get; set; }
    public string Status { get; set; } = null!;
}
