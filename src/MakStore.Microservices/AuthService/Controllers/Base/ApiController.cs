using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers.Base;

[ApiController]
[Route("api/v{version:apiVersion}")]
public class ApiController : ControllerBase
{
}