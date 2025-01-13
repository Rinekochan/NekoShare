using Microsoft.AspNetCore.Mvc;
using server.Helpers;

namespace server.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase { }