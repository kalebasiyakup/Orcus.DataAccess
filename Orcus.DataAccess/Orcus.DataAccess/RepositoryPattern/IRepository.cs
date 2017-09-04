using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Orcus.DataAccess
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IRepository<T> GetRepository<T>() where T : class;

        IList<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        IList<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        TEntity GetById(object id);

        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, object>>[] includes);

        TEntity Insert(TEntity entity);

        TEntity Update(TEntity entity);

        void Delete(object id);

        void Delete(TEntity entity);

        void Delete(Expression<Func<TEntity, bool>> filter);

        int GetCount(Expression<Func<TEntity, bool>> filter = null);

        bool GetExists(Expression<Func<TEntity, bool>> filter = null);
    }
}