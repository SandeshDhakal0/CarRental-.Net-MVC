using System.Linq.Expressions;

namespace eTickets.Data.Base;

public interface IEntityBaseRepository<T> where T : class, IEntityBase, new()
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
    Task<T> GetCarDetailAsync(int id);
    Task<T> GetCarDetailAsync(int id, params Expression<Func<T, object>>[] includeProperties);
    Task AddAsync(T entity);
    Task UpdateAsync(int id, T entity);
    Task DeleteAsync(int id);
}