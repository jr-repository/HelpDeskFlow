using HelpDeskFlow.Models;
using HelpDeskFlow.Repositories;

namespace HelpDeskFlow.Services;

public class DashboardService
{
    private readonly IJsonRepository<Ticket> _tickets;
    private readonly IJsonRepository<User> _users;
    private readonly IJsonRepository<Department> _departments;
    private readonly IJsonRepository<TicketComment> _comments;
    private readonly IJsonRepository<ActivityLog> _activityLogs;

    public DashboardService(
        IJsonRepository<Ticket> tickets,
        IJsonRepository<User> users,
        IJsonRepository<Department> departments,
        IJsonRepository<TicketComment> comments,
        IJsonRepository<ActivityLog> activityLogs)
    {
        _tickets = tickets;
        _users = users;
        _departments = departments;
        _comments = comments;
        _activityLogs = activityLogs;
    }

    public async Task<object> GetSummaryAsync()
    {
        var tickets = await _tickets.GetAllAsync();
        var users = await _users.GetAllAsync();
        var departments = await _departments.GetAllAsync();
        var comments = await _comments.GetAllAsync();
        var activityLogs = await _activityLogs.GetAllAsync();

        return new
        {
            totals = new
            {
                tickets = tickets.Count,
                users = users.Count,
                departments = departments.Count,
                comments = comments.Count,
                activityLogs = activityLogs.Count
            },
            tickets = new
            {
                open = tickets.Count(ticket => ticket.Status == "Open"),
                inProgress = tickets.Count(ticket => ticket.Status == "In Progress"),
                resolved = tickets.Count(ticket => ticket.Status == "Resolved"),
                closed = tickets.Count(ticket => ticket.Status == "Closed"),
                urgent = tickets.Count(ticket => ticket.Priority == "Urgent"),
                high = tickets.Count(ticket => ticket.Priority == "High"),
                medium = tickets.Count(ticket => ticket.Priority == "Medium"),
                low = tickets.Count(ticket => ticket.Priority == "Low")
            },
            workload = users.Select(user => new
            {
                user.Id,
                user.FullName,
                assignedTickets = tickets.Count(ticket => ticket.AssignedAgentId == user.Id)
            })
        };
    }
}
