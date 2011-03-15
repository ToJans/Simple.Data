﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Simple.Data.TestHelper;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class DatabaseSchemaTests : DatabaseSchemaTestsBase
    {
        private static readonly string DatabasePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)),
            "Northwind.db");

        protected override Database GetDatabase()
        {
            return Database.OpenFile(DatabasePath);
        }

        [Test]
        public void TestTables()
        {
            Assert.AreEqual(1, Schema.Tables.Count(t => t.ActualName == "Customers"));
        }

        [Test]
        public void TestColumns()
        {
            var table = Schema.FindTable("Customers");
            Assert.AreEqual(1, table.Columns.Count(c => c.ActualName == "CustomerID"));
        }

        [Test]
        public void TestPrimaryKey()
        {
            Assert.AreEqual("CustomerID", Schema.FindTable("Customers").PrimaryKey[0]);
        }
    }
}
