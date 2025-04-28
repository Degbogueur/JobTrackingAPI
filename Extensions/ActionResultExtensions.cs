using JobTrackingAPI.Contracts.Results;
using Microsoft.AspNetCore.Mvc;

namespace JobTrackingAPI.Extensions;

public static class ActionResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new NoContentResult();
        }

        else
        {
            return result.ErrorType switch
            {
                Enums.ErrorType.Conflict => new ConflictObjectResult(new { error = result.ErrorMessage }),
                Enums.ErrorType.ValidationError => new BadRequestObjectResult(new { error = result.ErrorMessage }),
                Enums.ErrorType.Unauthorized => new UnauthorizedObjectResult(new { error = result.ErrorMessage }),
                Enums.ErrorType.Forbidden => new ForbidResult(),
                Enums.ErrorType.NotFound => new NotFoundObjectResult(new { error = result.ErrorMessage }),
                _ => new ObjectResult(new { error = result.ErrorMessage })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
            };
        }
    }


    public static IActionResult ToActionResult<T>(this Result<T> result, string? createdAt = null)
    {
        if (result.IsSuccess)
        {
            if (result.Data is null)
            {
                return new NoContentResult();
            }

            if (createdAt is not null)
            {
                return new CreatedAtActionResult(
                    createdAt,
                    null,
                    new { id = (result.Data as dynamic)?.Id },
                    result.Data);
            }

            return new OkObjectResult(result.Data);
        }
        else
        {
            return result.ErrorType switch
            {
                Enums.ErrorType.Conflict => new ConflictObjectResult(new { error = result.ErrorMessage }),
                Enums.ErrorType.ValidationError => new BadRequestObjectResult(new { error = result.ErrorMessage }),
                Enums.ErrorType.Unauthorized => new UnauthorizedObjectResult(new { error = result.ErrorMessage }),
                Enums.ErrorType.Forbidden => new ForbidResult(),
                Enums.ErrorType.NotFound => new NotFoundObjectResult(new { error = result.ErrorMessage }),
                _ => new ObjectResult(new { error = result.ErrorMessage })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
            };
        }
    }
}
