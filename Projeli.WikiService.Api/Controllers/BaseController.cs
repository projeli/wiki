﻿using Microsoft.AspNetCore.Mvc;
using Projeli.Shared.Domain.Results;

namespace Projeli.WikiService.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult(IResult<object?> result)
    {
        if (result.Success)
        {
            return Ok(result);
        }

        if (result.Errors.Count > 0)
        {
            var errors = result.Errors
                .ToDictionary(x => x.Key, x => x.Value);

            return ValidationProblem(new ValidationProblemDetails
            {
                Title = result.Message,
                Errors = errors
            });
        }

        return BadRequest(result);
    }
}