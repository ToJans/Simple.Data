using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Simple.Data.SqliteTests
{
    [TestFixture]
    public class OrderDetailTests
    {
        private static readonly string DatabasePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)),
            "Northwind.db");

        [Test]
        public void TestOrderDetail()
        {
            var db = Database.OpenFile(DatabasePath);
            db.Orders.Insert(OrderID: 1000, OrderDate: DateTime.Today);
            var order = db.Orders.FindByOrderDate(new DateTime(1994, 11, 16));
            Assert.IsNotNull(order);
            var orderItem = order.OrderItems.FirstOrDefault();
            Assert.IsNotNull(orderItem);
            var item = orderItem.Item;
            Assert.AreEqual("Widget", item.Name);
        }
    }
}