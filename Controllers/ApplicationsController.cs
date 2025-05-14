using JobTrackingAPI.Contracts.Services;
using JobTrackingAPI.DTOs;
using JobTrackingAPI.Extensions;
using JobTrackingAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobTrackingAPI.Controllers;

[Route("api/applications")]
[ApiController]
public class ApplicationsController(
    IApplicationService applicationService) : ControllerBase
{
    private readonly IApplicationService _applicationService = applicationService;

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateApplicationDto createDto)
    {
        var result = await _applicationService.CreateAsync(createDto);
        return result.ToActionResult(nameof(GetById));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] QueryParameters? parameters = null)
    {
        parameters ??= new QueryParameters();
        var result = await _applicationService.GetAllAsync(parameters);
        return result.ToActionResult();
    }

    [HttpGet("trash")]
    public async Task<IActionResult> GetAllDeleted(
        [FromQuery] QueryParameters? parameters = null)
    {
        parameters ??= new QueryParameters();
        var result = await _applicationService.GetAllDeletedAsync(parameters);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _applicationService.GetByIdAsync(id);
        return result.ToActionResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateApplicationDto updateDto)
    {
        var result = await _applicationService.UpdateAsync(id, updateDto);
        return result.ToActionResult(nameof(GetById));
    }

    [HttpPatch("{id}/update-status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateApplicationStatusDto updateStatusDto)
    {
        var result = await _applicationService.UpdateStatusAsync(id, updateStatusDto);
        return result.ToActionResult(nameof(GetById));
    }

    [HttpPatch("{id}/delete")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var result = await _applicationService.SoftDeleteAsync(id);
        return result.ToActionResult();
    }

    [HttpPatch("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var result = await _applicationService.RestoreAsync(id);
        return result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _applicationService.DeleteAsync(id);
        return result.ToActionResult();
    }
}