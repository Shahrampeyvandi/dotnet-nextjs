using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using testttt.Application.Interfaces;
using testttt.Infrastructure.Data;

namespace testttt.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ECommerceDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ECommerceDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        // SaveChanges در Service layer انجام می‌شود
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        // SaveChanges در Service layer انجام می‌شود
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        // SaveChanges در Service layer انجام می‌شود
        await Task.CompletedTask;
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}

