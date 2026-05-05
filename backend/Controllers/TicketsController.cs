using HelpDeskFlow.Contracts;
using HelpDeskFlow.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskFlow.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketsController : ControllerBase
{
    private readonly TicketService _service;

    public TicketsController(TicketService service)
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
        var ticket = await _service.GetByIdAsync(id);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTicketRequest request)
    {
        try
        {
            var ticket = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTicketRequest request)
    {
        var ticket = await _service.UpdateAsync(id, request);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpPatch("{id:guid}/assign")]
    public async Task<IActionResult> Assign(Guid id, AssignTicketRequest request)
    {
        try
        {
            var ticket = await _service.AssignAsync(id, request);
            return ticket is null ? NotFound() : Ok(ticket);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, ChangeTicketStatusRequest request)
    {
        var ticket = await _service.ChangeStatusAsync(id, request);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpGet("{id:guid}/comments")]
    public async Task<IActionResult> GetComments(Guid id)
    {
        return Ok(await _service.GetCommentsAsync(id));
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid id, CreateTicketCommentRequest request)
    {
        try
        {
            return Ok(await _service.AddCommentAsync(id, request));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
