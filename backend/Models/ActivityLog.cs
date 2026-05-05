namespace HelpDeskFlow.Models;

public class ActivityLog : EntityBase
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
