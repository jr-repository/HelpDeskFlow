using HelpDeskFlow.Contracts;
using HelpDeskFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskFlow.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly DepartmentService _service;

    public DepartmentsController(DepartmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var department = await _service.GetByIdAsync(id);
        return department is null ? NotFound() : Ok(department);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDepartmentRequest request)
    {
        var department = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateDepartmentRequest request)
    {
        var department = await _service.UpdateAsync(id, request);
        return department is null ? NotFound() : Ok(department);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
