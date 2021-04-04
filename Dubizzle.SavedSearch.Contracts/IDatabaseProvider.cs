using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dubizzle.SavedSearch.Contracts
{
    public interface IDatabaseProvider
    {
        T Create<T>(T entity);
        T GetById<T>(Expression<Func<T, bool>> filter) where T : IEntity;
        IEnumerable<T> GetAll<T>(Expression<Func<T, bool>> filter) where T : IEntity;
        T Update<T>(T entity, Expression<Func<T, bool>> filter) where T : IEntity;
        void Delete<T>(Expression<Func<T, bool>> filter) where T : IEntity;
    }
}