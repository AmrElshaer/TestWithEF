using MediatR;
using Microsoft.AspNetCore.Mvc;
using TestWithEF.Filiters;
using TestWithEF.Models;

namespace TestWithEF.Controllers;

[ApiController]
[ApiExceptionFilter]
[Route("api/[controller]")]
public class ApiControllerBase : ControllerBase
{
    private ISender _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();
}

public static class ValidationToActionResult
{
    public static Task<IActionResult> ToActionResult<T>(this Task<Result<T>> result)
        => result.MatchAsync<IActionResult, T>(
            succ: t => new OkObjectResult(t),
            fail: e => new BadRequestObjectResult(e));
}
