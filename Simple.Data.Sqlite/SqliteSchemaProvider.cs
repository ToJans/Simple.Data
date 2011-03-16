using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
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

        public IConnectionProvider ConnectionProvider
        {
            get { return _connectionProvider; }
        }

        public IEnumerable<Table> GetTables()
        {
            return GetSchema("TABLES").Select(SchemaRowToTable);
        }

        private static Table SchemaRowToTable(DataRow row)
        {
            return new Table(row["TABLE_NAME"].ToString(), row["TABLE_SCHEMA"].ToString(),
                        row["TABLE_TYPE"].ToString() == "BASE TABLE" ? TableType.Table : TableType.View);
        }

        private IEnumerable<DataRow> GetSchema(string collectionName, params string[] constraints)
        {
            using (var cn = ConnectionProvider.CreateConnection())
            {
                cn.Open();

                return cn.GetSchema(collectionName, constraints).AsEnumerable();
            }
        }

        public IEnumerable<Column> GetColumns(Table table)
        {
            return GetColumnsDataTable(table).AsEnumerable().Select( 
                        row => new Column(row.Field<string>("name"), table, Convert.ToBoolean(row.Field<long>("pk"))));
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
            return new Key(GetColumns(table).Where(column => column.IsIdentity).Select(x => x.ActualName));
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

        private DataTable GetColumnsDataTable(Table table)
        {
            return SelectToDataTable("pragma table_info(" + table.ActualName + ");");
        }

        private DataTable SelectToDataTable(string sql)
        {
            var dataTable = new DataTable();
            using (var cn = ConnectionProvider.CreateConnection() as SQLiteConnection)
            {
                using (var adapter = new SQLiteDataAdapter(sql, cn))
                {
                    adapter.Fill(dataTable);
                }

            }

            return dataTable;
        }
    }
}