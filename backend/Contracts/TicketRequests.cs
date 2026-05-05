namespace HelpDeskFlow.Contracts;

public record CreateTicketRequest(string Subject, string Description, string Priority, Guid DepartmentId, Guid RequesterId);
public record UpdateTicketRequest(string Subject, string Description, string Priority, string Status, Guid DepartmentId, Guid RequesterId, Guid? AssignedAgentId);
public record AssignTicketRequest(Guid AgentId);
public record ChangeTicketStatusRequest(string Status);
public record CreateTicketCommentRequest(Guid UserId, string Message, bool IsInternalNote);
