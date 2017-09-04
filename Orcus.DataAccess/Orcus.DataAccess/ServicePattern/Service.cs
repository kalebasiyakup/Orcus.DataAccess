using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Orcus.DataAccess
{
    public abstract class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        #region Private Fields
        private readonly IRepository<TEntity> _repository;
        #endregion Private Fields

        #region Constructor
        protected Service(IUnitOfWork unitOfWork)
        {
            _repository = unitOfWork.Repository<TEntity>();
        }
        #endregion Constructor

        #region Methods
        public virtual Result<IList<TEntity>> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            Result<IList<TEntity>> result;
            try
            {
                result = new Result<IList<TEntity>>(_repository.GetAll(orderBy, skip, take, includeProperties));
            }
            catch (Exception ex)
            {
                result = new Result<IList<TEntity>>(ex);
            }

            return result;
        }

        public virtual Result<IList<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            Result<IList<TEntity>> result;
            try
            {
                result = new Result<IList<TEntity>>(_repository.Get(filter, orderBy, skip, take, includeProperties));
            }
            catch (Exception ex)
            {
                result = new Result<IList<TEntity>>(ex);
            }

            return result;
        }

        public Result<IQueryable<TEntity>> Query(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            Result<IQueryable<TEntity>> result;
            try
            {
                result = new Result<IQueryable<TEntity>>(_repository.Query(filter, orderBy));
            }
            catch (Exception ex)
            {
                result = new Result<IQueryable<TEntity>>(ex);
            }

            return result;
        }

        public Result<TEntity> GetById(object id)
        {
            Result<TEntity> result;
            try
            {
                result = new Result<TEntity>(_repository.GetById(id));
            }
            catch (Exception ex)
            {
                result = new Result<TEntity>(ex);
            }

            return result;
        }

        public Result<TEntity> GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            Result<TEntity> result;
            try
            {
                result = new Result<TEntity>(_repository.GetFirstOrDefault(filter, includes));
            }
            catch (Exception ex)
            {
                result = new Result<TEntity>(ex);
            }

            return result;
        }

        public virtual Result<TEntity> Insert(TEntity entity)
        {
            Result<TEntity> result;
            try
            {
                result = new Result<TEntity>(_repository.Insert(entity));
            }
            catch (Exception ex)
            {
                result = new Result<TEntity>(ex);
            }

            return result;
        }

        public virtual Result<TEntity> Update(TEntity entity)
        {
            Result<TEntity> result;
            try
            {
                result = new Result<TEntity>(_repository.Update(entity));
            }
            catch (Exception ex)
            {
                result = new Result<TEntity>(ex);
            }

            return result;
        }

        public virtual Result<bool> Delete(object id)
        {
            Result<bool> result;
            try
            {
                _repository.Delete(id);
                result = new Result<bool>(true);
            }
            catch (Exception ex)
            {
                result = new Result<bool>(ex);
            }

            return result;
        }

        public virtual Result<bool> Delete(TEntity entity)
        {
            Result<bool> result;
            try
            {
                _repository.Delete(entity);
                result = new Result<bool>(true);
            }
            catch (Exception ex)
            {
                result = new Result<bool>(ex);
            }

            return result;
        }

        public virtual Result<bool> Delete(Expression<Func<TEntity, bool>> filter)
        {
            Result<bool> result;
            try
            {
                _repository.Delete(filter);
                result = new Result<bool>(true);
            }
            catch (Exception ex)
            {
                result = new Result<bool>(ex);
            }

            return result;
        }

        public Result<int> GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            Result<int> result;
            try
            {
                result = new Result<int>(_repository.GetCount(filter));
            }
            catch (Exception ex)
            {
                result = new Result<int>(ex);
            }

            return result;
        }

        public Result<bool> GetExists(Expression<Func<TEntity, bool>> filter = null)
        {
            Result<bool> result;
            try
            {
                result = new Result<bool>(_repository.GetExists(filter));
            }
            catch (Exception ex)
            {
                result = new Result<bool>(ex);
            }

            return result;
        }

        #endregion
    }
}