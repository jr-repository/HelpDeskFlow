namespace HelpDeskFlow.Models;

public class TicketComment : EntityBase
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsInternalNote { get; set; }
}
