using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

namespace Orcus.DataAccess
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;
        private string _errorMessage;

        public EfRepository(DbContext dataContext, IUnitOfWork unitOfWork)
        {
            _dataContext = dataContext;
            _unitOfWork = unitOfWork;
        }

        private DbSet<TEntity> Dbset => _dataContext.Set<TEntity>();

        public IList<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetQueryable(null, orderBy, skip, take, includeProperties);
        }

        public IList<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetQueryable(filter, orderBy, skip, take, includeProperties);
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = Dbset;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return query;
        }

        public virtual TEntity GetById(object id)
        {
            return Dbset.Find(id);
        }

        public virtual TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Dbset;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            return filter != null ? query.FirstOrDefault(filter) : query.FirstOrDefault();
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).Count();
        }

        public virtual bool GetExists(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).Any();
        }

        protected virtual IList<TEntity> GetQueryable(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = Dbset;

            foreach (Expression<Func<TEntity, object>> include in includeProperties)
                query = query.Include(include);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query.ToList();
        }

        public virtual TEntity Insert(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw GetDbEntityValidationExceptionError(null, "Insert(T entity) - entity is null");
                }

                return Dbset.Add(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                throw GetDbEntityValidationExceptionError(dbEx, "Insert(T entity)");
            }
        }

        public virtual TEntity Update(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw GetDbEntityValidationExceptionError(null, "Update(T entity) - entity is null");
                }

                var updateEntity = _dataContext.Entry(entity);
                updateEntity.State = EntityState.Modified;
                return updateEntity.Entity;
            }
            catch (DbEntityValidationException dbEx)
            {
                throw GetDbEntityValidationExceptionError(dbEx, "Update(T entity)");
            }
        }

        public virtual void Delete(object id)
        {
            try
            {
                TEntity entity = Dbset.Find(id);

                if (entity == null)
                {
                    throw GetDbEntityValidationExceptionError(null, "Delete(T entity) - entity is null");
                }

                if (_dataContext.Entry(entity).State == EntityState.Detached)
                    Dbset.Attach(entity);
                Dbset.Remove(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                throw GetDbEntityValidationExceptionError(dbEx, "Delete(T entity)");
            }
        }

        public virtual void Delete(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw GetDbEntityValidationExceptionError(null, "Delete(T entity) - entity is null");
                }

                var entry = _dataContext.Entry(entity);
                if (entry.State == EntityState.Detached)
                    Dbset.Attach(entity);
                Dbset.Remove(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                throw GetDbEntityValidationExceptionError(dbEx, "Delete(T entity)");
            }
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> filter)
        {
            TEntity entity = GetFirstOrDefault(filter);
            if (entity != null)
            {
                try
                {
                    var entry = _dataContext.Entry(entity);
                    if (entry.State == EntityState.Detached)
                        Dbset.Attach(entity);
                    Dbset.Remove(entity);
                }
                catch (DbEntityValidationException dbEx)
                {
                    throw GetDbEntityValidationExceptionError(dbEx, "Delete(T entity)");
                }
            }
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return _unitOfWork.Repository<T>();
        }

        private Exception GetDbEntityValidationExceptionError(DbEntityValidationException dbEx, string methodName)
        {
            Exception exception = new Exception(methodName);

            if (dbEx == null)
            {
                return exception;
            }

            foreach (var validationError in dbEx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors))
            {
                _errorMessage += $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}" + Environment.NewLine;
            }

            exception.Data.Add("Method Name", methodName);
            exception.Data.Add("Log Detail", _errorMessage);

            return exception;
        }
    }
}