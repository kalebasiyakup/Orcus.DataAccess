using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace Orcus.DataAccess
{
    public class DataOperation : IDisposable
    {
        #region Private Fields
        private SqlConnection _oSqlConnection;
        private SqlTransaction _transaction;
        #endregion

        #region Constructor
        public DataOperation() { }
        public DataOperation(string connectionStringName)
        {
            _oSqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
        }
        #endregion

        #region Methods
        public bool IsConnectionOpen()
        {
            return _oSqlConnection != null && (_oSqlConnection.State != ConnectionState.Closed && _oSqlConnection.State != ConnectionState.Broken);
        }

        public void OpenConnection()
        {
            if (IsConnectionOpen()) return;

            try
            {
                _oSqlConnection.Open();
            }
            catch (Exception exception)
            {
                throw new Exception("OpenConnection: " + exception.Message, exception);
            }
        }

        public void OpenConnection(string connectionString)
        {
            if (IsConnectionOpen()) return;

            try
            {
                _oSqlConnection = new SqlConnection(connectionString);
                _oSqlConnection.Open();
            }
            catch (Exception exception)
            {
                string exMessage = exception.Message;
                if (!string.IsNullOrEmpty(connectionString))
                {
                    var ind1 = connectionString.IndexOf(";Password=", StringComparison.Ordinal);

                    if (ind1 > -1)
                    {
                        exMessage += " ConnectionString:" + connectionString.Substring(0, ind1);
                    }
                }


                throw new Exception("OpenConnection: " + exMessage, exception);
            }
        }

        public void CloseConnection()
        {
            if (!IsConnectionOpen()) return;

            try
            {
                if (_transaction != null)
                {
                    var oTransactionException = new TransactionException
                    {
                        Source = TransactionStatus.Active.ToString()
                    };

                    throw oTransactionException;
                }

                _oSqlConnection.Close();
            }
            catch (Exception exception)
            {
                throw new Exception("CloseConnection: " + exception.Message, exception);
            }
        }

        public void BeginTransaction()
        {
            if (IsConnectionOpen())
            {
                if (_transaction != null) return;

                try
                {
                    _transaction = _oSqlConnection.BeginTransaction();
                }
                catch (Exception exception)
                {
                    throw new Exception("BeginTransaction: " + exception.Message, exception);
                }
            }
            else
            {
                var oTransactionException = new TransactionException
                {
                    Source = ConnectionState.Closed.ToString()
                };

                throw oTransactionException;
            }
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Commit();
                }
                catch (Exception exception)
                {
                    throw new Exception("CommitTransaction: " + exception.Message, exception);
                }
                finally
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
            else
            {
                var oTransactionException = new TransactionException
                {
                    Source = TransactionStatus.Committed.ToString()
                };

                throw oTransactionException;
            }
        }

        public void RollBackTransaction()
        {
            if (_transaction == null) return;

            try
            {
                _transaction.Rollback();

            }
            catch (Exception exception)
            {
                throw new Exception("RollBackTransaction: " + exception.Message, exception);
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void PrepareCommand(SqlCommand oSqlCommand, CommandType oCommandType = CommandType.StoredProcedure)
        {
            oSqlCommand.Connection = _oSqlConnection;
            oSqlCommand.CommandType = oCommandType;

            if (_transaction != null)
            {
                oSqlCommand.Transaction = _transaction;
            }
        }

        public DataTable ExecuteDataTable(SqlCommand oSqlCommand)
        {
            PrepareCommand(oSqlCommand);

            OpenConnection();

            try
            {
                using (var oSqlDataAdapter = new SqlDataAdapter(oSqlCommand))
                {
                    using (var oDataTable = new DataTable())
                    {
                        oSqlDataAdapter.Fill(oDataTable);
                        return oDataTable;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("ExecuteDataTable: " + exception.Message, exception);
            }
        }

        public int ExecuteNonQuery(SqlCommand oSqlCommand)
        {
            PrepareCommand(oSqlCommand);

            OpenConnection();

            try
            {
                return oSqlCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new Exception("ExecuteNonQuery: " + exception.Message, exception);
            }
        }

        public object ExecuteScalar(SqlCommand oSqlCommand)
        {
            PrepareCommand(oSqlCommand);

            OpenConnection();

            try
            {
                return oSqlCommand.ExecuteScalar();
            }
            catch (Exception exception)
            {
                throw new Exception("ExecuteScalar: " + exception.Message, exception);
            }
        }

        public SqlDataReader ExecuteReader(SqlCommand oSqlCommand)
        {
            PrepareCommand(oSqlCommand);

            OpenConnection();

            try
            {
                return oSqlCommand.ExecuteReader();
            }
            catch (Exception exception)
            {
                throw new Exception("ExecuteReader: " + exception.Message, exception);
            }
        }
        #endregion

        #region Dispose pattern

        private bool _isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;

            if (isDisposing)
            {
                _oSqlConnection.Dispose();
            }
            _oSqlConnection = null;

            _isDisposed = true;
        }

        ~DataOperation()
        {
            Dispose(false);
        }

        #endregion
    }
}