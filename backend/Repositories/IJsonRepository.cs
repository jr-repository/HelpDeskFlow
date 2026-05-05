using HelpDeskFlow.Models;

namespace HelpDeskFlow.Repositories;

public interface IJsonRepository<T> where T : EntityBase
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task<T?> UpdateAsync(Guid id, T entity);
    Task<bool> DeleteAsync(Guid id);
}
