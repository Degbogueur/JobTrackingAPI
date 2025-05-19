using JobTrackingAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTrackingAPI.Data.Configurations;

public class ApplicationConfigurations : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.HasIndex(x => x.CompanyName);

        builder.HasIndex(x => x.Location);

        builder.HasIndex(x => x.JobTitle);

        builder.HasIndex(x => x.ApplicationDate);
    }
}