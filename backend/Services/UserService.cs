using HelpDeskFlow.Contracts;
using HelpDeskFlow.Models;
using HelpDeskFlow.Repositories;

namespace HelpDeskFlow.Services;

public class UserService
{
    private readonly IJsonRepository<User> _users;
    private readonly IJsonRepository<Department> _departments;

    public UserService(IJsonRepository<User> users, IJsonRepository<Department> departments)
    {
        _users = users;
        _departments = departments;
    }

    public Task<List<User>> GetAllAsync() => _users.GetAllAsync();

    public Task<User?> GetByIdAsync(Guid id) => _users.GetByIdAsync(id);

    public async Task<User> CreateAsync(CreateUserRequest request)
    {
        var department = await _departments.GetByIdAsync(request.DepartmentId);

        if (department is null)
        {
            throw new InvalidOperationException("Department not found.");
        }

        return await _users.AddAsync(new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Role = request.Role,
            DepartmentId = request.DepartmentId,
            IsActive = request.IsActive
        });
    }

    public async Task<User?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var department = await _departments.GetByIdAsync(request.DepartmentId);

        if (department is null)
        {
            throw new InvalidOperationException("Department not found.");
        }

        return await _users.UpdateAsync(id, new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Role = request.Role,
            DepartmentId = request.DepartmentId,
            IsActive = request.IsActive
        });
    }

    public Task<bool> DeleteAsync(Guid id) => _users.DeleteAsync(id);
}
