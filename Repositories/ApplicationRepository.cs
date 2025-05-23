﻿using JobTrackingAPI.Contracts.Repositories;
using JobTrackingAPI.Data;
using JobTrackingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTrackingAPI.Repositories;

public class ApplicationRepository(
    JobTrackingDbContext context) : IApplicationRepository
{
    private readonly JobTrackingDbContext _context = context;

    public async Task<Application> CreateAsync(
        Application application)
    {
        await _context.Applications.AddAsync(application);

        return application;
    }

    public void Delete(Application application)
    {
        _context.Applications.Remove(application);
    }

    public async Task<bool> ExistsAsync(
        string jobTitle, 
        string companyName,
        string location,
        int? excludedId)
    {
        return await _context.Applications.AnyAsync(a => 
            a.JobTitle == jobTitle &&
            a.CompanyName == companyName &&
            a.Location == location &&
            (excludedId == null || a.Id != excludedId)
        );
    }

    public IQueryable<Application> GetAll()
    {
        return _context.Applications;
    }

    public async Task<Application?> GetByIdAsync(int id)
    {
        return await _context.Applications.FindAsync(id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}