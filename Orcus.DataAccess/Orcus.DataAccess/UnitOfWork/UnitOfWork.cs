using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace Orcus.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Private Fields
        private ObjectContext _objectContext;
        private DbTransaction _transaction;
        private Dictionary<string, dynamic> _repositories;
        private DbContext _dataContext;
        private bool _disposed;
        #endregion Private Fields

        #region Constuctor/Dispose
        public UnitOfWork(DbContext dataContext)
        {
            _dataContext = dataContext;
            _repositories = new Dictionary<string, dynamic>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                try
                {
                    if (_objectContext != null && _objectContext.Connection.State == ConnectionState.Open)
                    {
                        _objectContext.Connection.Close();
                    }
                }
                catch (ObjectDisposedException)
                {
                }

                if (_dataContext != null)
                {
                    _dataContext.Dispose();
                    _dataContext = null;
                }
            }

            _disposed = true;
        }
        #endregion Constuctor/Dispose

        #region Methods
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<string, dynamic>();
            }

            var type = typeof(TEntity).Name;

            if (_repositories.ContainsKey(type))
            {
                return (EfRepository<TEntity>)_repositories[type];
            }
            var repositoryType = typeof(EfRepository<>);

            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _dataContext, this);

            _repositories.Add(type, repositoryInstance);

            return (EfRepository<TEntity>)_repositories[type];
        }

        public int SaveChanges()
        {
            return _dataContext.SaveChanges();
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            _objectContext = ((IObjectContextAdapter)_dataContext).ObjectContext;
            if (_objectContext.Connection.State != ConnectionState.Open)
            {
                _objectContext.Connection.Open();
            }

            _transaction = _objectContext.Connection.BeginTransaction(isolationLevel);
        }

        public bool CommitTransaction()
        {
            _transaction.Commit();
            return true;
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
        }
        #endregion
    }
}