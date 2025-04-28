using JobTrackingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTrackingAPI.Data;

public class JobTrackingDbContext(DbContextOptions options) 
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(JobTrackingDbContext).Assembly);
        base.OnModelCreating(builder);
    }

    public DbSet<Application> Applications { get; set; }
}
