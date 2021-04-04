using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Contracts
{
    public interface IDatabaseProvider
    {
        Task<T> CreateAsync<T>(T entity);
        Task DeleteAsync<T>(Expression<Func<T, bool>> filter) where T : IEntity;
        Task<IEnumerable<T>> GetAllAsync<T>(Expression<Func<T, bool>> filter) where T : IEntity;
        Task<T> GetByIdAsync<T>(Expression<Func<T, bool>> filter) where T : IEntity;
        Task<T> UpdateAsync<T>(T entity, Expression<Func<T, bool>> filter) where T : IEntity;
    }
}