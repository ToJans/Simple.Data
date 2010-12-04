using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Simple.Data
{
    internal class SimpleQueryBase : DynamicObject
    {
        private DataStrategy _dataStrategy;
        private string _tableName;
        private SimpleExpression _criteria;

        public SimpleQueryBase(DataStrategy dataStrategy, string tableName, SimpleExpression criteria)
        {
            _dataStrategy = dataStrategy;
            _criteria = criteria;
            _tableName = tableName;
        }

        public DataStrategy DataStrategy
        {
            get {
                return _dataStrategy;
            }
        }

        public string TableName
        {
            get {
                return _tableName;
            }
        }

        protected SimpleExpression Criteria
        {
            get { return _criteria; }
        }
    }

    class SimpleOneRowQuery : SimpleQueryBase
    {
        public SimpleOneRowQuery(DataStrategy dataStrategy, string tableName, SimpleExpression criteria) : base(dataStrategy, tableName, criteria)
        {
        }

        public DynamicRecord Run()
        {
            var row = DataStrategy.FindOne(TableName, Criteria);
            return row == null ? null : new DynamicRecord(row);
        }
    }
}
