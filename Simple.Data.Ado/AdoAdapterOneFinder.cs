using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Simple.Data.Ado
{
    class AdoAdapterOneFinder
    {
        private readonly AdoAdapter _adapter;
        private readonly DbTransaction _transaction;
        private readonly DbConnection _connection;

        public AdoAdapterOneFinder(AdoAdapter adapter) : this(adapter, null)
        {
        }

        public AdoAdapterOneFinder(AdoAdapter adapter, DbTransaction transaction)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            _adapter = adapter;

            if (transaction != null)
            {
                _transaction = transaction;
                _connection = transaction.Connection;
            }
        }

        public IDictionary<string, object> FindOne(string tableName, SimpleExpression criteria)
        {
            if (criteria == null) throw new ArgumentNullException("criteria");

            var commandBuilder = new FindHelper(_adapter.GetSchema()).GetFindByCommand(TableName.Parse(tableName), criteria);
            return ExecuteQuery(commandBuilder);
        }

        private IDictionary<string, object> ExecuteQuery(ICommandBuilder commandBuilder)
        {
            var connection = _connection ?? _adapter.CreateConnection();
            var command = commandBuilder.GetCommand(connection);
            command.Transaction = _transaction;
            return TryExecuteQuery(command);
        }

        private static IDictionary<string, object> TryExecuteQuery(IDbCommand command)
        {
            try
            {
                if (command.Connection.State == ConnectionState.Open)
                {
                    return ReadSingleRecord(command);
                }

                using (command.Connection)
                {
                    command.Connection.Open();
                    return ReadSingleRecord(command);
                }
            }
            catch (DbException ex)
            {
                throw new AdoAdapterException(ex.Message, command);
            }
        }

        private static IDictionary<string, object> ReadSingleRecord(IDbCommand command)
        {
            using (command)
            using (var reader = command.ExecuteReader())
            {
                return reader.Read() ? reader.ToDictionary() : null;
            }
        }
    }
}
