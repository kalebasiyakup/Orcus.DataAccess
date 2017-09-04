using System;
using System.Data;

namespace Orcus.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
        int SaveChanges();
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        bool CommitTransaction();
        void RollbackTransaction();
        void Dispose(bool disposing);
    }
}