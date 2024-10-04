using Microsoft.AspNetCore.Mvc;

namespace MakStore.SharedComponents.Api;

[ApiController]
[Route("api/v{version:apiVersion}")]
public class ApiController : ControllerBase
{
}