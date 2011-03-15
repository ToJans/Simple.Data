using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;

namespace Simple.Data.Sqlite
{
    [Export("db", typeof(IConnectionProvider))]
    public class SqliteConnectionProvider : IConnectionProvider
    {
        string _connectionString;

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public ISchemaProvider GetSchemaProvider()
        {
            return new SqliteSchemaProvider(this);
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public bool SupportsCompoundStatements
        {
            get { return true; }
        }

        public string GetIdentityFunction()
        {
            return "SELECT last_insert_rowid() AS ID;";
        }

        public bool SupportsStoredProcedures
        {
            get { return false; }
        }

        public IProcedureExecutor GetProcedureExecutor(AdoAdapter adapter, ObjectName procedureName)
        {
            throw new NotSupportedException("Sqlite does not support stored procedures.");
        }
    }
}
