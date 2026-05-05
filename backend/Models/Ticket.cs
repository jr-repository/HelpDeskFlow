namespace HelpDeskFlow.Models;

public class Ticket : EntityBase
{
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Open";
    public Guid DepartmentId { get; set; }
    public Guid RequesterId { get; set; }
    public Guid? AssignedAgentId { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
