using System;
using System.Data;

namespace SharpQuant.Common.DB
{
    public class Transaction : IDbTransaction
    {
        Action _commit;
        Action _rollback;
        Action _dispose;
        IDbConnection _conn;
        IsolationLevel _isolationLevel;

        public Transaction(Action commit, Action rollback, Action dispose, 
            IsolationLevel il = System.Data.IsolationLevel.Unspecified, IDbConnection conn=null)
        {
            _commit = commit;
            _rollback = rollback;
            _dispose = dispose;
            _isolationLevel = IsolationLevel;
            _conn = conn;
        }

        public void Commit()
        {
            if (_commit!=null) _commit();
        }

        public IDbConnection Connection
        {
            get { return _conn; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return _isolationLevel; }
        }

        public void Rollback()
        {
            if (_rollback != null) _rollback();
        }

        public void Dispose()
        {
            if (_dispose != null) _dispose();
        }
    }
}
