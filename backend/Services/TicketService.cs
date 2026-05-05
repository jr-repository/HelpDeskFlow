using HelpDeskFlow.Contracts;
using HelpDeskFlow.Models;
using HelpDeskFlow.Repositories;

namespace HelpDeskFlow.Services;

public class TicketService
{
    private readonly IJsonRepository<Ticket> _tickets;
    private readonly IJsonRepository<TicketComment> _comments;
    private readonly IJsonRepository<ActivityLog> _activityLogs;
    private readonly IJsonRepository<User> _users;
    private readonly IJsonRepository<Department> _departments;

    public TicketService(
        IJsonRepository<Ticket> tickets,
        IJsonRepository<TicketComment> comments,
        IJsonRepository<ActivityLog> activityLogs,
        IJsonRepository<User> users,
        IJsonRepository<Department> departments)
    {
        _tickets = tickets;
        _comments = comments;
        _activityLogs = activityLogs;
        _users = users;
        _departments = departments;
    }

    public Task<List<Ticket>> GetAllAsync() => _tickets.GetAllAsync();

    public Task<Ticket?> GetByIdAsync(Guid id) => _tickets.GetByIdAsync(id);

    public async Task<Ticket> CreateAsync(CreateTicketRequest request)
    {
        var requester = await _users.GetByIdAsync(request.RequesterId);

        if (requester is null)
        {
            throw new InvalidOperationException("Requester not found.");
        }

        var department = await _departments.GetByIdAsync(request.DepartmentId);

        if (department is null)
        {
            throw new InvalidOperationException("Department not found.");
        }

        var ticket = await _tickets.AddAsync(new Ticket
        {
            TicketNumber = $"TCK-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Subject = request.Subject,
            Description = request.Description,
            Priority = request.Priority,
            DepartmentId = request.DepartmentId,
            RequesterId = request.RequesterId,
            Status = "Open"
        });

        await AddActivityAsync("Ticket", ticket.Id, "Created", $"Ticket {ticket.TicketNumber} was created.");

        return ticket;
    }

    public async Task<Ticket?> UpdateAsync(Guid id, UpdateTicketRequest request)
    {
        var existing = await _tickets.GetByIdAsync(id);

        if (existing is null)
        {
            return null;
        }

        var ticket = new Ticket
        {
            TicketNumber = existing.TicketNumber,
            Subject = request.Subject,
            Description = request.Description,
            Priority = request.Priority,
            Status = request.Status,
            DepartmentId = request.DepartmentId,
            RequesterId = request.RequesterId,
            AssignedAgentId = request.AssignedAgentId,
            ResolvedAt = request.Status == "Resolved" ? DateTime.UtcNow : existing.ResolvedAt
        };

        var updated = await _tickets.UpdateAsync(id, ticket);

        if (updated is not null)
        {
            await AddActivityAsync("Ticket", id, "Updated", $"Ticket {existing.TicketNumber} was updated.");
        }

        return updated;
    }

    public async Task<Ticket?> AssignAsync(Guid id, AssignTicketRequest request)
    {
        var ticket = await _tickets.GetByIdAsync(id);

        if (ticket is null)
        {
            return null;
        }

        var agent = await _users.GetByIdAsync(request.AgentId);

        if (agent is null)
        {
            throw new InvalidOperationException("Agent not found.");
        }

        ticket.AssignedAgentId = request.AgentId;
        ticket.Status = "In Progress";

        var updated = await _tickets.UpdateAsync(id, ticket);

        if (updated is not null)
        {
            await AddActivityAsync("Ticket", id, "Assigned", $"Ticket {ticket.TicketNumber} was assigned.");
        }

        return updated;
    }

    public async Task<Ticket?> ChangeStatusAsync(Guid id, ChangeTicketStatusRequest request)
    {
        var ticket = await _tickets.GetByIdAsync(id);

        if (ticket is null)
        {
            return null;
        }

        ticket.Status = request.Status;
        ticket.ResolvedAt = request.Status == "Resolved" ? DateTime.UtcNow : ticket.ResolvedAt;

        var updated = await _tickets.UpdateAsync(id, ticket);

        if (updated is not null)
        {
            await AddActivityAsync("Ticket", id, "StatusChanged", $"Ticket {ticket.TicketNumber} status changed to {request.Status}.");
        }

        return updated;
    }

    public async Task<TicketComment> AddCommentAsync(Guid ticketId, CreateTicketCommentRequest request)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId);

        if (ticket is null)
        {
            throw new InvalidOperationException("Ticket not found.");
        }

        var user = await _users.GetByIdAsync(request.UserId);

        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        var comment = await _comments.AddAsync(new TicketComment
        {
            TicketId = ticketId,
            UserId = request.UserId,
            Message = request.Message,
            IsInternalNote = request.IsInternalNote
        });

        await AddActivityAsync("Ticket", ticketId, "CommentAdded", $"A comment was added to ticket {ticket.TicketNumber}.");

        return comment;
    }

    public async Task<List<TicketComment>> GetCommentsAsync(Guid ticketId)
    {
        var comments = await _comments.GetAllAsync();
        return comments.Where(comment => comment.TicketId == ticketId).ToList();
    }

    public Task<bool> DeleteAsync(Guid id) => _tickets.DeleteAsync(id);

    private Task<ActivityLog> AddActivityAsync(string entityType, Guid entityId, string action, string description)
    {
        return _activityLogs.AddAsync(new ActivityLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            Description = description
        });
    }
}
