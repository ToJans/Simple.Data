using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace Simple.Data
{
    class SimpleQuery : DynamicObject, IEnumerable
    {
        private readonly DataStrategy _dataStrategy;
        private readonly string _tableName;
        private readonly SimpleExpression _criteria;
        private IEnumerable<DynamicRecord> _enumerable;

        public SimpleQuery(DataStrategy dataStrategy, string tableName, SimpleExpression criteria)
        {
            _dataStrategy = dataStrategy;
            _criteria = criteria;
            _tableName = tableName;
        }

        public IEnumerable<T> Cast<T>()
        {
            return GetEnumerable().Select(item => (T)item);
        }

        public IEnumerable<T> OfType<T>()
        {
            foreach (var item in GetEnumerable())
            {
                bool success = true;
                T cast;
                try
                {
                    cast = (T)item;
                }
                catch (RuntimeBinderException)
                {
                    cast = default(T);
                    success = false;
                }
                catch (InvalidCastException)
                {
                    cast = default(T);
                    success = false;
                }
                if (success)
                {
                    yield return cast;
                }
            }
        }

        public IList<dynamic> ToList()
        {
            return GetEnumerable().ToList();
        }

        public dynamic[] ToArray()
        {
            return GetEnumerable().ToArray();
        }

        public IList<T> ToList<T>()
        {
            return Cast<T>().ToList();
        }

        public T[] ToArray<T>()
        {
            return Cast<T>().ToArray();
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (ConvertIsToEnumerable(binder))
            {
                result = Cast<dynamic>();
                return true;
            }
            result = null;
            return false;
        }

        private static bool ConvertIsToEnumerable(ConvertBinder binder)
        {
            return binder.Type.IsGenericType
                   && binder.Type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator GetEnumerator()
        {
            return (_enumerable ?? Run()).GetEnumerator();
        }

        private IEnumerable<dynamic> GetEnumerable()
        {
            return (_enumerable ?? Run());
        }

        private IEnumerable<DynamicRecord> Run()
        {
            return _enumerable = _dataStrategy.Find(_tableName, _criteria)
                .Select(d => new DynamicRecord(d, _tableName, _dataStrategy));
        }
    }
}
