using HelpDeskFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskFlow.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _service;

    public DashboardController(DashboardService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        return Ok(await _service.GetSummaryAsync());
    }
}
