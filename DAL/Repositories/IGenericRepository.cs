using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace TaskManagementSystem.DAL.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null); // To send delegete (=>)
        Task<T> GetByIdAsync(object id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRange(IEnumerable<T> entities);
        Task SaveAsync();
    }
}
