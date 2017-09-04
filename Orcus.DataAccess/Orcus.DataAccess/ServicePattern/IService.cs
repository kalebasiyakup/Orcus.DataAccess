using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Orcus.DataAccess
{
    public interface IService<TEntity>
    {
        Result<IList<TEntity>> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        Result<IList<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        Result<IQueryable<TEntity>> Query(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        Result<TEntity> GetById(object id);

        Result<TEntity> GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, object>>[] includes);

        Result<TEntity> Insert(TEntity entity);

        Result<TEntity> Update(TEntity entity);

        Result<bool> Delete(object id);

        Result<bool> Delete(TEntity entity);

        Result<bool> Delete(Expression<Func<TEntity, bool>> filter);

        Result<int> GetCount(Expression<Func<TEntity, bool>> filter = null);

        Result<bool> GetExists(Expression<Func<TEntity, bool>> filter = null);
    }
}