using BlogCMS.Domain.Common;
using BlogCMS.Domain.Entities;
using System.Linq.Expressions;

namespace BlogCMS.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<bool> ExistsAsync(Guid id);
    Task<int> SaveChangesAsync();
    //void FindAsync(Func<Tag, bool> func);
} 