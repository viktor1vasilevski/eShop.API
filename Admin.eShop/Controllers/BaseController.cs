﻿using eShop.Main.Responses;

namespace Admin.eShop.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResponse<T>(ApiResponse<T> response) where T : class
    {
        return response.NotificationType switch
        {
            NotificationType.Success => Ok(response),
            NotificationType.BadRequest => BadRequest(response),
            NotificationType.NotFound => NotFound(response),
            NotificationType.Created => StatusCode(201, response),
            NotificationType.NoContent => NoContent(),
            NotificationType.ServerError => StatusCode(500, response),
            NotificationType.Conflict => StatusCode(409, response),
            NotificationType.Unauthorized => Unauthorized(response),
            _ => Ok(response),
        };
    }
}
