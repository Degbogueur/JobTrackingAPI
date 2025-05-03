using JobTrackingAPI.Enums;
using JobTrackingAPI.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace JobTrackingAPI.Controllers;

[Route("api/enums")]
[ApiController]
public class EnumsController : ControllerBase
{
    [HttpGet("action-types")]
    public IActionResult GetActionTypes()
    {
        var jobStatuses = EnumMappers.EnumToList<ActionType>();
        return Ok(jobStatuses);
    }

    [HttpGet("application-statuses")]
    public IActionResult GetApplicationStatuses()
    {
        var applicationStatuses = EnumMappers.EnumToList<ApplicationStatus>();
        return Ok(applicationStatuses);
    }

    [HttpGet("contract-types")]
    public IActionResult GetContractTypes()
    {
        var contractTypes = EnumMappers.EnumToList<ContractType>();
        return Ok(contractTypes);
    }

    [HttpGet("currencies")]
    public IActionResult GetCurrencies()
    {
        var currencies = EnumMappers.EnumToList<Currency>();
        return Ok(currencies);
    }

    [HttpGet("job-sources")]
    public IActionResult GetJobSources()
    {
        var jobSources = EnumMappers.EnumToList<JobSource>();
        return Ok(jobSources);
    }

    [HttpGet("priorities")]
    public IActionResult GetPriorities()
    {
        var priorities = EnumMappers.EnumToList<Priority>();
        return Ok(priorities);
    }

    [HttpGet("rejection-reasons")]
    public IActionResult GetRejectionReasons()
    {
        var rejectionReasons = EnumMappers.EnumToList<RejectionReason>();
        return Ok(rejectionReasons);
    }

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var allEnums = new
        {
            ActionTypes = EnumMappers.EnumToList<ActionType>(),
            ApplicationStatuses = EnumMappers.EnumToList<ApplicationStatus>(),
            ContractTypes = EnumMappers.EnumToList<ContractType>(),
            Currencies = EnumMappers.EnumToList<Currency>(),
            JobSources = EnumMappers.EnumToList<JobSource>(),
            Priorities = EnumMappers.EnumToList<Priority>(),
            RejectionReasons = EnumMappers.EnumToList<RejectionReason>()
        };
        return Ok(allEnums);
    }
}
