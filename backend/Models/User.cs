namespace HelpDeskFlow.Models;

public class User : EntityBase
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Agent";
    public Guid DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
}
