using System;
using System.Collections.Generic;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;

namespace Simple.Data.Sqlite
{
    class SqliteSchemaProvider : ISchemaProvider
    {
        IConnectionProvider _connectionProvider;

        public SqliteSchemaProvider(SqliteConnectionProvider connectionProvider)
        {
            if (connectionProvider == null) throw new ArgumentNullException("connectionProvider");
            _connectionProvider = connectionProvider;
        }

        public IEnumerable<Table> GetTables()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Column> GetColumns(Table table)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Procedure> GetStoredProcedures()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Parameter> GetParameters(Procedure storedProcedure)
        {
            throw new NotImplementedException();
        }

        public Key GetPrimaryKey(Table table)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ForeignKey> GetForeignKeys(Table table)
        {
            throw new NotImplementedException();
        }

        public string QuoteObjectName(string unquotedName)
        {
            throw new NotImplementedException();
        }

        public string NameParameter(string baseName)
        {
            throw new NotImplementedException();
        }
    }
}