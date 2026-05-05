using HelpDeskFlow.Contracts;
using HelpDeskFlow.Models;
using HelpDeskFlow.Repositories;

namespace HelpDeskFlow.Services;

public class DepartmentService
{
    private readonly IJsonRepository<Department> _departments;

    public DepartmentService(IJsonRepository<Department> departments)
    {
        _departments = departments;
    }

    public Task<List<Department>> GetAllAsync() => _departments.GetAllAsync();

    public Task<Department?> GetByIdAsync(Guid id) => _departments.GetByIdAsync(id);

    public Task<Department> CreateAsync(CreateDepartmentRequest request)
    {
        return _departments.AddAsync(new Department
        {
            Name = request.Name,
            Description = request.Description
        });
    }

    public Task<Department?> UpdateAsync(Guid id, UpdateDepartmentRequest request)
    {
        return _departments.UpdateAsync(id, new Department
        {
            Name = request.Name,
            Description = request.Description
        });
    }

    public Task<bool> DeleteAsync(Guid id) => _departments.DeleteAsync(id);
}
