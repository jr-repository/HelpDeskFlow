namespace HelpDeskFlow.Contracts;

public record CreateUserRequest(string FullName, string Email, string Role, Guid DepartmentId, bool IsActive);
public record UpdateUserRequest(string FullName, string Email, string Role, Guid DepartmentId, bool IsActive);
