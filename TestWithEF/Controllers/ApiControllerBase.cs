using Microsoft.AspNetCore.Mvc;
using TestWithEF.Filiters;

namespace TestWithEF.Controllers;

[ApiController]
[ApiExceptionFilter]
[Route("api/[controller]")]
public class ApiControllerBase : ControllerBase { }
