using System.Text.Json;
using HelpDeskFlow.Models;

namespace HelpDeskFlow.Repositories;

public class JsonRepository<T> : IJsonRepository<T> where T : EntityBase
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonRepository(IWebHostEnvironment environment)
    {
        var dataDirectory = Path.Combine(environment.ContentRootPath, "Data");
        Directory.CreateDirectory(dataDirectory);
        _filePath = Path.Combine(dataDirectory, $"{typeof(T).Name}.json");
    }

    public async Task<List<T>> GetAllAsync()
    {
        await _lock.WaitAsync();

        try
        {
            return await ReadUnsafeAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var items = await GetAllAsync();
        return items.FirstOrDefault(item => item.Id == id);
    }

    public async Task<T> AddAsync(T entity)
    {
        await _lock.WaitAsync();

        try
        {
            var items = await ReadUnsafeAsync();

            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            items.Add(entity);
            await WriteUnsafeAsync(items);

            return entity;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<T?> UpdateAsync(Guid id, T entity)
    {
        await _lock.WaitAsync();

        try
        {
            var items = await ReadUnsafeAsync();
            var index = items.FindIndex(item => item.Id == id);

            if (index < 0)
            {
                return null;
            }

            entity.Id = id;
            entity.CreatedAt = items[index].CreatedAt;
            entity.UpdatedAt = DateTime.UtcNow;

            items[index] = entity;
            await WriteUnsafeAsync(items);

            return entity;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await _lock.WaitAsync();

        try
        {
            var items = await ReadUnsafeAsync();
            var removed = items.RemoveAll(item => item.Id == id) > 0;

            if (!removed)
            {
                return false;
            }

            await WriteUnsafeAsync(items);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<T>> ReadUnsafeAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<T>();
        }

        var content = await File.ReadAllTextAsync(_filePath);

        if (string.IsNullOrWhiteSpace(content))
        {
            return new List<T>();
        }

        return JsonSerializer.Deserialize<List<T>>(content, _jsonOptions) ?? new List<T>();
    }

    private async Task WriteUnsafeAsync(List<T> items)
    {
        var content = JsonSerializer.Serialize(items, _jsonOptions);
        await File.WriteAllTextAsync(_filePath, content);
    }
}
