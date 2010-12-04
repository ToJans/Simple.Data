using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using Simple.Data.Extensions;

namespace Simple.Data
{
    public partial class DynamicRecord : DynamicObject
    {
        private readonly SimpleOneRowQuery _query;
        private readonly ConcreteObject _concreteObject = new ConcreteObject();
        private readonly HomogenizedKeyDictionary _data;
        private readonly DataStrategy _dataStrategy;
        private readonly string _tableName;
        private bool? _null;

        internal DynamicRecord()
        {
            _null = false;
            _data = new HomogenizedKeyDictionary();
        }

        internal DynamicRecord(Database database)
        {
            _data = new HomogenizedKeyDictionary();
            _dataStrategy = database;
            _null = false;
        }

        internal DynamicRecord(IEnumerable<KeyValuePair<string, object>> data) : this(data, null)
        {
        }

        internal DynamicRecord(IEnumerable<KeyValuePair<string, object>> data, string tableName)
            : this(data, tableName, null)
        {
        }

        internal DynamicRecord(IEnumerable<KeyValuePair<string, object>> data, string tableName, DataStrategy dataStrategy)
        {
            _tableName = tableName;
            _dataStrategy = dataStrategy;
            _data = new HomogenizedKeyDictionary(data);
            _null = false;
        }

        internal DynamicRecord(SimpleOneRowQuery query)
        {
            _query = query;
            _dataStrategy = _query.DataStrategy;
            _tableName = _query.TableName;
            _data = new HomogenizedKeyDictionary();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_null.HasValue)
            {
                if (_null.Value) throw new NullReferenceException();
                if (_data.ContainsKey(binder.Name))
                {
                    result = _data[binder.Name];
                    return true;
                }
            }
            if (AmNull()) throw new NullReferenceException();
            if (_data.ContainsKey(binder.Name))
            {
                result = _data[binder.Name];
                return true;
            }
            var relatedAdapter = _dataStrategy.Adapter as IAdapterWithRelation;
            if (relatedAdapter != null && relatedAdapter.IsValidRelation(_tableName, binder.Name))
            {
                var relatedRows = relatedAdapter.FindRelated(_tableName, _data, binder.Name);

                if (relatedRows.Count() == 1 && !binder.Name.IsPlural())
                {
                    result = new DynamicRecord(relatedRows.Single(), binder.Name, _dataStrategy);
                }
                else
                {
                    result = new DynamicEnumerable(relatedRows.Select(dict => new DynamicRecord(dict, binder.Name, _dataStrategy)));
                }
                return true;
            }
            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (AmNull()) throw new NullReferenceException();
            _data[binder.Name.Homogenize()] = value;
            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (AmNull()) throw new NullReferenceException();
            result = _concreteObject.Get(binder.Type, _data);
            return result != null;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _data.Keys.AsEnumerable();
        }

        internal DynamicRecord Fetch()
        {
            return AmNull() ? null : this;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (AmNull() && ReferenceEquals(obj, null)) return true;
            return base.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(DynamicRecord other)
        {
            if (ReferenceEquals(null, other)) return AmNull();
            if (ReferenceEquals(this, other)) return true;
            return other.AmNull().Equals(AmNull());
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            if (AmNull()) throw new NullReferenceException();
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (AmNull()) throw new NullReferenceException();
            return base.ToString();
        }

        public static bool operator ==(DynamicRecord left, DynamicRecord right)
        {
            if (ReferenceEquals(left, null)) return ReferenceEquals(right, null) || right.AmNull();
            if (ReferenceEquals(right, null)) return left.AmNull();
            return Equals(left, right);
        }

        public static bool operator !=(DynamicRecord left, DynamicRecord right)
        {
            return !Equals(left, right);
        }

        private bool AmNull()
        {
            if (_null.HasValue) return _null.Value;
            var data = _query.Run();
            _null = (data == null);
            if (!_null.Value)
            {
                _data.AddRange(data);
            }
            return _null.Value;
        }

        private bool AmNotNull()
        {
            return !AmNull();
        }
    }
}