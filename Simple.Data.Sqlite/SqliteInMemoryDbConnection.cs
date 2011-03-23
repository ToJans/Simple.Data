using System.Data;
using Simple.Data.Ado;

namespace Simple.Data.Sqlite
{
    public class SqliteInMemoryDbConnection : DelegatingConnectionBase
    {
        public SqliteInMemoryDbConnection(IDbConnection target) : base(target)
        { }

        public override void Open()
        {
            if(_target.State == ConnectionState.Closed)
                _target.Open();
        }

        public override void Close()
        {
            //do not close explicitly
        }

        public DataTable GetSchema(string collectionName, params string[] constraints)
        {
            return _target.GetSchema(collectionName, constraints);
        }

        public override void Dispose()
        {
            //do not dispose anything...
        }

    }
}