using System.Data.SQLite;
using NUnit.Framework;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class InMemoryUsageTests
    {
        const string connectionString = "Data Source=:memory:";
        SQLiteConnection connection;

        [SetUp]
        public void SetUp()
        {
            var createTableSql = Properties.Resources.CreateEmployeesTable;
            connection = new SQLiteConnection(connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = createTableSql;
            command.ExecuteNonQuery();
            
        }

        [TearDown]
        public void TearDown()
        {
            connection.Close();
            connection.Dispose();
            connection = null;
        }

        [Test]
        public void CanCreateInMemoryDatabase()
        {
            Assert.Pass();
        }

        [Test]
        public void CanQueryInMemoryDatabase()
        {
            Assert.That(connection, Is.Not.Null);
            var db = Database.OpenConnection(connectionString);
            var employees = db.Employees.All();
            Assert.That(employees.Count(), Is.EqualTo(3));
        }

        [Test]
        public void CanInsertInMemoryDatabase()
        {
            var db = Database.OpenConnection(connectionString);
            var employee = db.Employees.Insert(EmpName: "Dirk Diggler", EmpSalary: 100000);
            Assert.That(employee.EmpName, Is.EqualTo("Dirk Diggler"));
            Assert.That(employee.EmpSalary, Is.EqualTo(100000));
            Assert.That(employee.EmpID, Is.GreaterThan(0));
        }
    }
}